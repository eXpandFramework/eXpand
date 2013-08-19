using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ConditionalObjectViewRuleController : ViewController {
        LogicRuleViewController _logicRuleViewController;
        IModelView _defaultObjectView;

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing+=FrameOnDisposing;
            _logicRuleViewController = Frame.GetController<LogicRuleViewController>();
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute += LogicRuleViewControllerOnLogicRuleExecute;
        }

        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing-=FrameOnDisposing;
            _logicRuleViewController.LogicRuleExecutor.LogicRuleExecute -= LogicRuleViewControllerOnLogicRuleExecute;
        }


        public class InfoController:ViewController {
            public InfoController(bool active) {
                Active["context"] = active;
            }

            public InfoController() {
                Active["context"] = false;
            }

            public IModelView Model { get; set; }
        }

        void LogicRuleViewControllerOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            LogicRuleInfo info = logicRuleExecuteEventArgs.LogicRuleInfo;
            if (info.InvertCustomization)
                return;
            var objectViewRule = info.Rule as IObjectViewRule;
            if (objectViewRule!=null) {
                ExecutionContext executionContext = logicRuleExecuteEventArgs.ExecutionContext;
                switch (executionContext) {
                    case ExecutionContext.None:
                        if (info.Active) {
                            ProcessActions(info, objectViewRule);
                        }
                        break;
                    case ExecutionContext.CustomProcessSelectedItem:
                        if (info.Active && objectViewRule.ObjectView is IModelListView) {
                            CustomProcessSelectedItem(info, objectViewRule);
                        }
                        break;
                    case ExecutionContext.CustomizeShowViewParameters:
                        if (info.Active&&objectViewRule.ObjectView is IModelDetailView) {
                            CustomizeShowViewParameters(info, objectViewRule);
                        }
                        break;
                    case ExecutionContext.CurrentObjectChanged:
                        if (View.Model.AsObjectView is IModelDetailView && objectViewRule.ObjectView is IModelDetailView) {
                            View.SetModel(info.Active ? objectViewRule.ObjectView : GetDefaultObjectView(),true);
                        }
                        break;
                }

            }
        }
        
        void ProcessActions(LogicRuleInfo info, IObjectViewRule objectViewRule) {
            var createdView = ((ActionBaseEventArgs)info.EventArgs).ShowViewParameters.CreatedView;
            if (createdView.Model.GetType() == objectViewRule.ObjectView.GetType())
                createdView.SetModel(objectViewRule.ObjectView);
        }

        void CustomProcessSelectedItem(LogicRuleInfo info, IObjectViewRule objectViewRule) {
            var customProcessListViewSelectedItemEventArgs = ((CustomProcessListViewSelectedItemEventArgs) info.EventArgs);
            var type = objectViewRule.ObjectView.ModelClass.TypeInfo.Type;
            var collectionSource = Application.CreateCollectionSource(Application.CreateObjectSpace(type), type,objectViewRule.ObjectView.Id);
            var showViewParameters = customProcessListViewSelectedItemEventArgs.InnerArgs.ShowViewParameters;
            showViewParameters.CreatedView = Application.CreateListView((IModelListView) objectViewRule.ObjectView,collectionSource, true);
            customProcessListViewSelectedItemEventArgs.Handled = true;
        }

        void CustomizeShowViewParameters(LogicRuleInfo info, IObjectViewRule objectViewRule) {
            var customizeShowViewParametersEventArgs = ((CustomizeShowViewParametersEventArgs) info.EventArgs);
            var createdView = customizeShowViewParametersEventArgs.ShowViewParameters.CreatedView;
            if (createdView is DetailView) {
                _defaultObjectView = createdView.Model;
                customizeShowViewParametersEventArgs.ShowViewParameters.Controllers.Add(new InfoController(true){
                    Model = _defaultObjectView
                });
                createdView.SetModel(objectViewRule.ObjectView);
            }
        }


        IModelView GetDefaultObjectView() {
            return _defaultObjectView??Frame.GetController<InfoController>().Model;
        }
    }
}