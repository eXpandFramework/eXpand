using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.Logic {
    public class LogicRuleExecutor {
        readonly LogicRuleEvaluator _evaluator = new LogicRuleEvaluator();
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecuted;
        public event EventHandler<LogicRuleExecutingEventArgs> LogicRuleExecuting;
        public event EventHandler<LogicRuleExecuteEventArgs> LogicRuleExecute;

        public LogicRuleEvaluator Evaluator {
            get { return _evaluator; }
        }

        public virtual void Execute(View view, bool invertCustomization, ExecutionContext executionContext, object currentObject, ActionBaseEventArgs args,EventArgs eventArgs) {
            var validRules = _evaluator.GetValidRules(view, executionContext);
            var logicRuleInfos = validRules.Select(o => new LogicRuleInfo{
                Active = _evaluator.Fit(currentObject, o),
                Object = currentObject,
                Rule = o,
                ExecutionContext = executionContext,
                View = view,
                ActionBaseEventArgs = args,
                EventArgs=eventArgs,
                InvertCustomization=invertCustomization
            });
            foreach (var logicRuleInfo in logicRuleInfos) {
                logicRuleInfo.ActionBaseEventArgs=args;
                ExecuteCore(logicRuleInfo, executionContext);
            }
        }

        public virtual void Execute(View view, bool invertCustomization, ExecutionContext executionContext, object currentObject,EventArgs eventArgs) {
            Execute(view, invertCustomization, executionContext, currentObject, null,eventArgs);
        }

        void ExecuteCore(LogicRuleInfo logicRuleInfo, ExecutionContext executionContext) {
            var args = new LogicRuleExecutingEventArgs(logicRuleInfo, false, executionContext);
            OnLogicRuleExecuting(args);
            if (!args.Cancel) {
                OnLogicRuleExecute(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
            }
            OnLogicRuleExecuted(new LogicRuleExecuteEventArgs(logicRuleInfo, executionContext));
        }

        protected virtual void OnLogicRuleExecute(LogicRuleExecuteEventArgs e) {
            EventHandler<LogicRuleExecuteEventArgs> handler = LogicRuleExecute;
            if (handler != null) handler(this, e);
        }

        public void InvertExecution(View view, ExecutionContext executionContext, object currentObject,EventArgs eventArgs) {
            Execute(view, true, executionContext, currentObject,eventArgs);
        }

        protected void InvertExecution(View view, ExecutionContext executionContext,EventArgs args) {
            InvertExecution(view, executionContext, View.CurrentObject,args);
        }

        public View View { get; set; }

        public void InvertAndExecute(View view, ExecutionContext executionContext, EventArgs eventArgs) {
            if (View != null) InvertExecution(View, executionContext,eventArgs);
            Execute(executionContext, view, eventArgs);
        }

        public void Execute(ActionBaseEventArgs args) {
            Execute(args.ShowViewParameters.CreatedView, false, ExecutionContext.None, args.ShowViewParameters.CreatedView.CurrentObject, args,EventArgs.Empty);
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

        public void Execute(ExecutionContext executionContext, View view, EventArgs eventArgs) {
            Execute(view, false, executionContext,view!=null? view.CurrentObject:null, eventArgs);
        }

        public void Execute(ExecutionContext executionContext,EventArgs eventArgs) {
            Execute(executionContext, View, eventArgs);
        }
    }
    public class LogicRuleViewController : ViewController {
        readonly LogicRuleExecutor _logicRuleExecutor=new LogicRuleExecutor();
        private bool isRefreshing;
        object _previousObject;
        XafApplication _application;
        ListViewProcessCurrentObjectController _listViewProcessCurrentObjectController;

        public LogicRuleExecutor LogicRuleExecutor {
            get { return _logicRuleExecutor; }
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            _logicRuleExecutor.Evaluator.Frame = Frame;
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged += FrameOnTemplateChanged;
            if (_application == null) {
                _application = Application;
                _application.ViewShowing += ApplicationOnViewShowing;
            }
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleExecutor.Evaluator.Frame = null;
        }

        void ApplicationOnViewShowing(object sender, ViewShowingEventArgs viewShowingEventArgs) {
            _logicRuleExecutor.InvertAndExecute(viewShowingEventArgs.View, ExecutionContext.ViewShowing,viewShowingEventArgs);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Frame.ViewChanging -= FrameOnViewChanging;
                Frame.TemplateChanged -= FrameOnTemplateChanged;
                if (_application != null) {
                    _application.ViewShowing -= ApplicationOnViewShowing;
                }
            }
            base.Dispose(disposing);
        }
        void FrameOnViewChanging(object sender, ViewChangingEventArgs args) {
            _logicRuleExecutor.View = args.View;
            _logicRuleExecutor.InvertAndExecute(args.View, ExecutionContext.ViewChanging, args);
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewChanged = (Frame.Template) as ISupportViewChanged;
            if (supportViewChanged != null)
                supportViewChanged.ViewChanged += (o, args) => _logicRuleExecutor.Execute(ExecutionContext.ViewChanged, args);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                SubscribeToActionEvents();
                View.SelectionChanged += ViewOnSelectionChanged;
                View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
                View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;

                ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
                ObjectSpace.Committed += ObjectSpaceOnCommitted;
                ObjectSpace.Refreshing += ObjectSpace_Refreshing;
                ObjectSpace.Reloaded += ObjectSpace_Reloaded;

                Frame.TemplateViewChanged += FrameOnTemplateViewChanged;
                _logicRuleExecutor.Execute(ExecutionContext.ControllerActivated,EventArgs.Empty);
                
                if (View is ListView) {
                    _listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                    _listViewProcessCurrentObjectController.CustomProcessSelectedItem += OnCustomProcessSelectedItem;
                    _listViewProcessCurrentObjectController.CustomizeShowViewParameters+=CustomizeShowViewParameters;
                }
            }
        }

        void SubscribeToActionEvents() {
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
        }

        void CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs customizeShowViewParametersEventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.CustomizeShowViewParameters, customizeShowViewParametersEventArgs.ShowViewParameters.CreatedView,customizeShowViewParametersEventArgs);
        }

        IEnumerable<IModelLogic> ModelLogics {
            get { return LogicInstallerManager.Instance.LogicInstallers.Select(installer => installer.GetModelLogic()); }
        }

        IEnumerable<ActionBase> GetActions() {
            var actionBases = Enumerable.Empty<ActionBase>();
            var actionExecutionContextGroups = ModelLogics.SelectMany(logic => logic.ActionExecutionContextGroup);
            return actionExecutionContextGroups.SelectMany(@group => @group, (@group, executionContext)
                => executionContext.Name).Aggregate(actionBases, (current, actionContexts)
                => current.Union(Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base
                    => actionContexts.Contains(@base.Id))));
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _logicRuleExecutor.Execute(actionBaseEventArgs);
        }

        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.ViewOnSelectionChanged,eventArgs);
        }

        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.CustomProcessSelectedItem, customProcessListViewSelectedItemEventArgs);
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceCommited, eventArgs);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            _logicRuleExecutor.Execute(ExecutionContext.ViewControlsCreated, EventArgs.Empty);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                UnsubscribeFromActionEvents();
                View.SelectionChanged -= ViewOnSelectionChanged;
                View.CurrentObjectChanged -= ViewOnCurrentObjectChanged;
                View.QueryCanChangeCurrentObject -= ViewOnQueryCanChangeCurrentObject;

                if (ObjectSpace != null) {
                    ObjectSpace.ObjectChanged -= ObjectSpaceOnObjectChanged;
                    ObjectSpace.Refreshing -= ObjectSpace_Refreshing;
                    ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
                    ObjectSpace.Committed -= ObjectSpaceOnCommitted;
                }

                Frame.TemplateViewChanged -= FrameOnTemplateViewChanged;
                if (View is ListView) {
                    _listViewProcessCurrentObjectController.CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
                    _listViewProcessCurrentObjectController.CustomizeShowViewParameters -= CustomizeShowViewParameters;
                }
            }
        }

        void UnsubscribeFromActionEvents() {
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
        }

        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            _previousObject = View.CurrentObject;
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.TemplateViewChanged, eventArgs);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isRefreshing = false;
            _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceReloaded, e);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e) {
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args) {
            if (_previousObject != null && !(ObjectSpace.IsDisposedObject(_previousObject))) {
                _logicRuleExecutor.InvertExecution(View, ExecutionContext.CurrentObjectChanged, _previousObject,args);
                var notifyPropertyChanged = _previousObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
            if (!isRefreshing) {
                _logicRuleExecutor.Execute(ExecutionContext.CurrentObjectChanged, args);
                var notifyPropertyChanged = View.CurrentObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            _logicRuleExecutor.Execute(View, false, ExecutionContext.NotifyPropertyObjectChanged, sender,propertyChangedEventArgs);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args) {
            if (!String.IsNullOrEmpty(args.PropertyName) && View != null)
                _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceObjectChanged, args);
        }        
    }
}