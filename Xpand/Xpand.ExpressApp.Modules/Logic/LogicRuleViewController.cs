using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.Logic {
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
            _logicRuleExecutor.InvertAndExecute(viewShowingEventArgs.View, ExecutionContext.ViewShowing,viewShowingEventArgs,View);
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
            _logicRuleExecutor.InvertAndExecute(args.View, ExecutionContext.ViewChanging, args,View);
        }

        void FrameOnTemplateChanged(object sender, EventArgs eventArgs) {
            var supportViewChanged = (Frame.Template) as ISupportViewChanged;
            if (supportViewChanged != null)
                supportViewChanged.ViewChanged += (o, args) => _logicRuleExecutor.Execute(ExecutionContext.ViewChanged, args,View);
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
                _logicRuleExecutor.Execute(ExecutionContext.ControllerActivated,EventArgs.Empty,View);
                
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

        IEnumerable<IModelLogicWrapper> ModelLogics {
            get { return LogicInstallerManager.Instance.LogicInstallers.Select(installer => installer.GetModelLogic(Application.Model)); }
        }

        IEnumerable<ActionBase> GetActions() {
            var actionBases = Enumerable.Empty<ActionBase>();
            var modelLogicWrappers = ModelLogics.Where(wrapper => wrapper.ActionExecutionContextGroup != null);
            var actionExecutionContextGroups = modelLogicWrappers.SelectMany(logic => logic.ActionExecutionContextGroup);
            return actionExecutionContextGroups.SelectMany(@group => @group, (@group, executionContext)
                => executionContext.Name).Aggregate(actionBases, (current, actionContexts)
                => current.Union(Frame.Controllers.Cast<Controller>().SelectMany(controller => controller.Actions).Where(@base
                    => actionContexts.Contains(@base.Id))));
        }

        void ActionOnExecuted(object sender, ActionBaseEventArgs actionBaseEventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.None, actionBaseEventArgs,View);
        }

        void ViewOnSelectionChanged(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.ViewOnSelectionChanged,eventArgs,View);
        }

        void OnCustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs customProcessListViewSelectedItemEventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.CustomProcessSelectedItem, customProcessListViewSelectedItemEventArgs,View);
        }

        void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs) {
            _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceCommited, eventArgs,View);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            _logicRuleExecutor.Execute(ExecutionContext.ViewControlsCreated, EventArgs.Empty,View);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                _logicRuleExecutor.Execute(ExecutionContext.ControllerDeActivated,EventArgs.Empty,View);
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
            _logicRuleExecutor.Execute(ExecutionContext.TemplateViewChanged, eventArgs,View);
        }

        private void ObjectSpace_Reloaded(object sender, EventArgs e) {
            isRefreshing = false;
            _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceReloaded, e,View);
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
                _logicRuleExecutor.Execute(ExecutionContext.CurrentObjectChanged, args,View);
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
                _logicRuleExecutor.Execute(ExecutionContext.ObjectSpaceObjectChanged, args,View);
        }        
    }
}