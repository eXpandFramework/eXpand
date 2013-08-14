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

        public virtual void ForceExecution(View view, bool invertCustomization, ExecutionContext executionContext, object currentObject, ActionBaseEventArgs args) {
            var ruleInfos = _evaluator.GetValidRuleInfos(view, currentObject, executionContext, invertCustomization, args);
            foreach (var logicRuleInfo in ruleInfos) {
                ForceExecutionCore(logicRuleInfo, executionContext);
            }
        }

        public virtual void ForceExecution(View view, bool invertCustomization, ExecutionContext executionContext, object currentObject) {
            ForceExecution(view, invertCustomization, executionContext, currentObject, null);
        }

        public virtual void ForceExecution(View view, bool invertCustomization, ExecutionContext executionContext) {
            ForceExecution(view, invertCustomization, executionContext, view == null ? null : view.CurrentObject);
        }

        void ForceExecutionCore(LogicRuleInfo logicRuleInfo, ExecutionContext executionContext) {
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

        public void InvertExecution(View view, ExecutionContext executionContext, object currentObject) {
            ForceExecution(view, true, executionContext, currentObject);
        }

        protected void InvertExecution(View view, ExecutionContext executionContext) {
            InvertExecution(view, executionContext, View.CurrentObject);
        }

        public View View { get; set; }

        public void ForceExecutioning(View view, ExecutionContext executionContext) {
            if (View != null) InvertExecution(View, executionContext);
            ForceExecution(view, false, executionContext);
        }

        public void ForceExecution(ActionBaseEventArgs args) {
            ForceExecution(args.ShowViewParameters.CreatedView, false, ExecutionContext.None, args.ShowViewParameters.CreatedView.CurrentObject, args);
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

        public void ForceExecution(ExecutionContext executionContext, View view) {
            ForceExecution(view, false, executionContext);
        }

        public void ForceExecution(ExecutionContext executionContext) {
            ForceExecution(executionContext, View);
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
            _logicRuleExecutor.Evaluator.Frame = Frame;
            Frame.ViewChanging += FrameOnViewChanging;
            Frame.TemplateChanged += FrameOnTemplateChanged;
            if (_application == null) {
                _application = Application;
                _application.ViewShowing += ApplicationOnViewShowing;
                _application.ViewCreating += ApplicationOnViewCreating;
            }
        }

        void ApplicationOnViewCreating(object sender, ViewCreatingEventArgs viewCreatingEventArgs) {
            _logicRuleExecutor.ForceExecution(viewCreatingEventArgs.View, false, ExecutionContext.ViewCreating);
        }

        void ApplicationOnViewShowing(object sender, ViewShowingEventArgs viewShowingEventArgs) {
            _logicRuleExecutor.ForceExecutioning(viewShowingEventArgs.View, ExecutionContext.ViewShowing);
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
            _logicRuleExecutor.ForceExecutioning(args.View, ExecutionContext.ViewChanging);
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewChanged = (Frame.Template) as ISupportViewChanged;
            if (supportViewChanged != null)
                supportViewChanged.ViewChanged += (o, args) => _logicRuleExecutor.ForceExecution(ExecutionContext.ViewChanged, args.View);
        }

        protected override void OnActivated() {
            base.OnActivated();
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
            _logicRuleExecutor.ForceExecution(ExecutionContext.ControllerActivated);
            View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            View.CurrentObjectChanged += ViewOnCurrentObjectChanged;
            View.QueryCanChangeCurrentObject += ViewOnQueryCanChangeCurrentObject;
            View.ObjectSpace.Refreshing += ObjectSpace_Refreshing;
            View.ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            if (View is ListView) {
                _listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem += OnCustomProcessSelectedItem;
                _listViewProcessCurrentObjectController.CustomizeShowViewParameters+=CustomizeShowViewParameters;
            }
        }

        void CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs customizeShowViewParametersEventArgs) {
            _logicRuleExecutor.ForceExecution(ExecutionContext.CustomizeShowViewParameters);
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
            _logicRuleExecutor.ForceExecution(actionBaseEventArgs);
        }

        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.ForceExecution(ExecutionContext.ViewOnSelectionChanged);
        }

        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            _logicRuleExecutor.ForceExecution(ExecutionContext.CustomProcessSelectedItem);
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.ForceExecution(ExecutionContext.ObjectSpaceCommited);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            _logicRuleExecutor.ForceExecution(ExecutionContext.ViewControlsCreated);
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

            if (View is ListView) {
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem -= OnCustomProcessSelectedItem;
                _listViewProcessCurrentObjectController.CustomizeShowViewParameters-=CustomizeShowViewParameters;
            }
        }

        void ViewOnQueryCanChangeCurrentObject(object sender, CancelEventArgs cancelEventArgs) {
            _previousObject = View.CurrentObject;
        }

        void FrameOnTemplateViewChanged(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.ForceExecution(ExecutionContext.TemplateViewChanged);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isRefreshing = false;
            _logicRuleExecutor.ForceExecution(ExecutionContext.ObjectSpaceReloaded);
        }

        private void ObjectSpace_Refreshing(object sender, CancelEventArgs e) {
            isRefreshing = true;
        }

        private void ViewOnCurrentObjectChanged(object sender, EventArgs args) {
            if (_previousObject != null && !(ObjectSpace.IsDisposedObject(_previousObject))) {
                _logicRuleExecutor.InvertExecution(View, ExecutionContext.CurrentObjectChanged, _previousObject);
                var notifyPropertyChanged = _previousObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            }
            if (!isRefreshing) {
                _logicRuleExecutor.ForceExecution(ExecutionContext.CurrentObjectChanged);
                var notifyPropertyChanged = View.CurrentObject as INotifyPropertyChanged;
                if (notifyPropertyChanged != null)
                    notifyPropertyChanged.PropertyChanged += OnPropertyChanged;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
            _logicRuleExecutor.ForceExecution(View, false, ExecutionContext.NotifyPropertyObjectChanged, sender);
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs args) {
            if (!String.IsNullOrEmpty(args.PropertyName) && View != null)
                _logicRuleExecutor.ForceExecution(ExecutionContext.ObjectSpaceObjectChanged);
        }        
    }
}