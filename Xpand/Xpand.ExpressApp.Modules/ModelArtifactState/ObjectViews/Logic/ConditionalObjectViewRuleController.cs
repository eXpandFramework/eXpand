using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Logic;
using Xpand.Persistent.Base.Logic;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Logic {
    public class ConditionalObjectViewRuleController : ViewController {
        LogicRuleViewController _logicRuleViewController;
        IModelDetailView _modelForCustomizeShowViewParameters;
        ListViewProcessCurrentObjectController _listViewProcessCurrentObjectController;
        IModelView _defaultObjectView;
        IModelListView _modelForCustomProcessSelectedItem;

        protected override void OnActivated() {
            base.OnActivated();
            if (View is ListView && LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                _listViewProcessCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
                _listViewProcessCurrentObjectController.CustomizeShowViewParameters += CustomizeShowViewParameters;
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem += CustomProcessSelectedItem;
            }
        }

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

        void CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e) {
            var createdView = e.ShowViewParameters.CreatedView;
            if (createdView is DetailView ) {
                _defaultObjectView = createdView.Model;
                e.ShowViewParameters.Controllers.Add(new InfoController(true){Model=_defaultObjectView});
                if (_modelForCustomizeShowViewParameters != null)
                    createdView.SetModel(_modelForCustomizeShowViewParameters);
                    
            }
            _modelForCustomizeShowViewParameters = null;
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
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (View is ListView && LogicRuleManager.HasRules(View.ObjectTypeInfo)) {
                _listViewProcessCurrentObjectController.CustomizeShowViewParameters -= CustomizeShowViewParameters;
                _listViewProcessCurrentObjectController.CustomProcessSelectedItem-=CustomProcessSelectedItem;
            }
        }

        void CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
            if (_modelForCustomProcessSelectedItem != null) {
                var type = _modelForCustomProcessSelectedItem.ModelClass.TypeInfo.Type;
                var collectionSource = Application.CreateCollectionSource(Application.CreateObjectSpace(type), type, _modelForCustomProcessSelectedItem.Id);
                e.InnerArgs.ShowViewParameters.CreatedView = Application.CreateListView(_modelForCustomProcessSelectedItem, collectionSource, true);
                e.Handled = true;
            }
            _modelForCustomProcessSelectedItem = null;
        }

        void LogicRuleViewControllerOnLogicRuleExecute(object sender, LogicRuleExecuteEventArgs logicRuleExecuteEventArgs) {
            LogicRuleInfo info = logicRuleExecuteEventArgs.LogicRuleInfo;
            if (info.InvertingCustomization)
                return;
            var objectViewRule = info.Rule as IObjectViewRule;
            if (objectViewRule!=null) {
                ExecutionContext executionContext = logicRuleExecuteEventArgs.ExecutionContext;
                switch (executionContext) {
                    case ExecutionContext.None:
                        if (info.Active && objectViewRule.ObjectView is IModelDetailView) {
                            var createdView = info.ActionBaseEventArgs.ShowViewParameters.CreatedView;
                            createdView.SetModel(objectViewRule.ObjectView);
                        }
                        break;
                    case ExecutionContext.CustomProcessSelectedItem:
                        if (info.Active && objectViewRule.ObjectView is IModelListView)
                            _modelForCustomProcessSelectedItem = ((IModelListView)objectViewRule.ObjectView);
                        break;
                    case ExecutionContext.CustomizeShowViewParameters:
                        if (info.Active&&objectViewRule.ObjectView is IModelDetailView)
                            _modelForCustomizeShowViewParameters = ((IModelDetailView)objectViewRule.ObjectView);
                        break;
                    case ExecutionContext.CurrentObjectChanged:
                        if (View.Model.AsObjectView is IModelDetailView) {
                            var defaultObjectView = GetDefaultObjectView();
                            View.SetModel(info.Active ? objectViewRule.ObjectView : defaultObjectView);
                        }
                        break;
                }

            }
        }

        IModelView GetDefaultObjectView() {
            return _defaultObjectView??Frame.GetController<InfoController>().Model;
        }
    }
}