using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleViewController : ViewController {
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecuted;
        public event EventHandler<LogicRuleExecutingEventArgs> LogicRuleExecuting;
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecute;
        private bool isRefreshing;
        //        public static readonly string ActiveObjectTypeHasRules = "ObjectTypeHas" + typeof(TModelLogicRule).Name;
        object _previousObject;
        XafApplication _application;
        readonly LogicRuleEvaluator _evaluator = new LogicRuleEvaluator();

        public virtual bool IsReady {
            get {
                return View != null && View.ObjectTypeInfo != null;
                //                return Active.ResultValue && View != null && View.ObjectTypeInfo != null;
            }
        }

        public LogicRuleEvaluator Evaluator {
            get { return _evaluator; }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext, object currentObject,
                                           ActionBase action) {
            if (isReady && view != null) {
                //                var modelLogicRules = GetValidModelLogicRules(view);
                //                var logicRuleInfos = GetContextValidLogicRuleInfos(view, modelLogicRules, currentObject, executionContext, invertCustomization, action);
                var ruleInfos = _evaluator.GetContextValidLogicRuleInfos(view, currentObject, executionContext, invertCustomization, action);
                foreach (var logicRuleInfo in ruleInfos) {
                    ForceExecutionCore(logicRuleInfo, executionContext);
                }
            }
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext, object currentObject) {
            ForceExecution(isReady, view, invertCustomization, executionContext, currentObject, null);
        }

        public virtual void ForceExecution(bool isReady, View view, bool invertCustomization, ExecutionContext executionContext) {
            ForceExecution(isReady, view, invertCustomization, executionContext, view == null ? null : view.CurrentObject);
        }

        void ForceExecutionCore(LogicRuleInfo logicRuleInfo, ExecutionContext executionContext) {
            var args = new LogicRuleExecutingEventArgs(logicRuleInfo, false, executionContext);
            OnLogicRuleExecuting(args);
            if (!args.Cancel) {
                OnLogicRuleExecute(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
            }
            OnLogicRuleExecuted(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
        }

        protected virtual void ExecuteRule(LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {

        }

        protected virtual void OnLogicRuleExecute(LogicRuleExecuteEventArgs e) {
            EventHandler<LogicRuleExecuteEventArgs> handler = LogicRuleExecute;
            if (handler != null) handler(this, e);
        }

        void InvertExecution(View view, ExecutionContext executionContext, object currentObject) {
            bool hasRules = LogicRuleManager.HasRules(view);
            ForceExecution(hasRules && view != null && view.ObjectTypeInfo != null, view, true, executionContext, currentObject);
        }

        protected void InvertExecution(View view, ExecutionContext executionContext) {
            InvertExecution(view, executionContext, View.CurrentObject);
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _evaluator.Frame = Frame;
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged += FrameOnTemplateChanged;
            if (_application == null) {
                _application = Application;
                _application.ViewShowing += ApplicationOnViewShowing;
                _application.ViewCreating += ApplicationOnViewCreating;
            }
        }

        void ApplicationOnViewCreating(object sender, ViewCreatingEventArgs viewCreatingEventArgs) {
            if (Application != null) {
                var modelObjectView = Application.Model.Views.Single(modelView => modelView.Id == viewCreatingEventArgs.ViewID).AsObjectView;
                if (modelObjectView != null) {
                    var typeInfo = modelObjectView.ModelClass.TypeInfo;
                    bool hasRules = LogicRuleManager.HasRules(typeInfo);
                    ForceExecution(hasRules, null, false, ExecutionContext.ViewCreating);
                }
            }
        }

        void ApplicationOnViewShowing(object sender, ViewShowingEventArgs viewShowingEventArgs) {
            if (Application != null)
                ForceExecutioning(viewShowingEventArgs.View, ExecutionContext.ViewShowing);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Frame.ViewChanging -= FrameOnViewChanging;
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                if (_application != null) {
                    _application.ViewShowing -= ApplicationOnViewShowing;
                    _application.ViewCreating -= ApplicationOnViewCreating;
                }
            }
            base.Dispose(disposing);
        }
        void FrameOnViewChanging(object sender, ViewChangingEventArgs args) {
            ForceExecutioning(args.View, ExecutionContext.ViewChanging);
        }

        void ForceExecutioning(View view, ExecutionContext executionContext) {
            if (View != null) InvertExecution(View, executionContext);
            bool hasRules = LogicRuleManager.HasRules(view);
            ForceExecution(hasRules && view != null && view.ObjectTypeInfo != null, view, false, executionContext);
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewChanged = (Frame.Template) as ISupportViewChanged;
            if (supportViewChanged != null)
                supportViewChanged.ViewChanged += (o, args) => {
                    //                    Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(args.View);
                    ForceExecution(ExecutionContext.ViewChanged, args.View);
                };
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady) {
                var actions = GetActions();
                foreach (var action in actions) {
                    var simpleAction = action as SimpleAction;
                    if (simpleAction != null)
                        simpleAction.Execute += ActionOnExecuted;
                    else {
                        var choiceAction = action as SingleChoiceAction;
                        if (choiceAction != null)
                            choiceAction.Execute += ActionOnExecuted;
                        else {
                            var parametrizedAction = action as ParametrizedAction;
                            if (parametrizedAction != null)
                                parametrizedAction.Execute += ActionOnExecuted;
                        }
                    }
                }
                View.SelectionChanged += ViewOnSelectionChanged;
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                ForceExecution(ExecutionContext.ControllerActivated);
                View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
                View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;
                View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
                if (View is ListView)
                    Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem += OnCustomProcessSelectedItem;
            }
        }

        IEnumerable<ActionBase> GetActions() {
            var actionBases = Enumerable.Empty<ActionBase>();
            var actionExecutionContextGroups = LogicRuleEvaluator.ModelLogics.SelectMany(logic => logic.ActionExecutionContextGroup);
            return actionExecutionContextGroups.SelectMany(@group => @group, (@group, executionContext)
                => executionContext.Name).Aggregate(actionBases, (current, actionContexts)
                => current.Union(Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base
                    => actionContexts.Contains(@base.Id))));
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            ForceExecution(actionBaseEventArgs);
        }

        void ForceExecution(ActionBaseEventArgs args) {
            //            Active[ActiveObjectTypeHasRules] = LogicRuleManager<TModelLogicRule>.HasRules(args.ShowViewParameters.CreatedView.ObjectTypeInfo);
            ForceExecution(IsReady, args.ShowViewParameters.CreatedView, false, ExecutionContext.None, args.ShowViewParameters.CreatedView.CurrentObject, args.Action);
        }


        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.ViewOnSelectionChanged);
        }


        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            ForceExecution(ExecutionContext.CustomProcessSelectedItem);
        }


        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.ObjectSpaceCommited);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            ForceExecution(ExecutionContext.ViewControlsCreated);
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            var actions = GetActions();
            foreach (var action in actions) {
                var simpleAction = action as SimpleAction;
                if (simpleAction != null)
                    simpleAction.Execute -= ActionOnExecuted;
                else {
                    var choiceAction = action as SingleChoiceAction;
                    if (choiceAction != null)
                        choiceAction.Execute -= ActionOnExecuted;
                    else {
                        var parametrizedAction = action as ParametrizedAction;
                        if (parametrizedAction != null)
                            parametrizedAction.Execute -= ActionOnExecuted;
                    }
                }
            }
            View.SelectionChanged -= ViewOnSelectionChanged;
            if (ObjectSpace != null) {
                ObjectSpace.Committed -= ObjectSpaceOnCommitted;
                View.ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                View.ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                View.ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            }
            Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;

            View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
            View.QueryCanChangeCurrentObject -= ViewOnQueryCanChangeCurrentObject;

            if (View is ListView)
                Frame.GetController<ListViewProcessCurrentObjectController>().CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
        }

        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            _previousObject = View.CurrentObject;
        }


        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            ForceExecution(ExecutionContext.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isRefreshing = false;
            if (View != null)
                ForceExecution(ExecutionContext.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e) {
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args) {
            if (_previousObject != null && !(ObjectSpace.IsDisposedObject(_previousObject))) {
                InvertExecution(View, ExecutionContext.CurrentObjectChanged, _previousObject);
                var notifyPropertyChanged = _previousObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
            if (!isRefreshing) {
                ForceExecution(ExecutionContext.CurrentObjectChanged);
                var notifyPropertyChanged = View.CurrentObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            ForceExecution(IsReady, View, false, ExecutionContext.NotifyPropertyObjectChanged, sender);
        }


        private void ForceExecution(ExecutionContext executionContext, View view) {
            ForceExecution(IsReady, view, false, executionContext);
        }

        private void ForceExecution(ExecutionContext executionContext) {
            ForceExecution(executionContext, View);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args) {
            if (!String.IsNullOrEmpty(args.PropertyName) && View != null)
                ForceExecution(ExecutionContext.ObjectSpaceObjectChanged);
        }

        protected virtual void OnLogicRuleExecuting(LogicRuleExecutingEventArgs args) {
            if (LogicRuleExecuting != null) {
                LogicRuleExecuting(this, args);
            }
        }

        protected virtual void OnLogicRuleExecuted(LogicRuleExecuteEventArgs args) {
            if (LogicRuleExecuted != null) {
                LogicRuleExecuted(this, args);
            }
        }
    }
}