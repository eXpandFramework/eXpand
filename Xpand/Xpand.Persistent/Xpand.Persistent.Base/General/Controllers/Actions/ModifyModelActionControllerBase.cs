using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

namespace Xpand.Persistent.Base.General.Controllers.Actions{
    public class ModifyModelActionChoiceItemsUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator>{
        public const string ChangeViewModel = "Change View Model";
        public const string ResetViewModel = "Reset View Model";

        public override void UpdateNode(ModelNode node) {
            var modelAction = ((IModelActions)node)[ActionModifyModelController.ModifyModelActionName];
            if (modelAction != null && modelAction.ChoiceActionItems != null) {
                modelAction.ChoiceActionItems.AddNode<IModelChoiceActionItem>(ResetViewModel);
                modelAction.ChoiceActionItems.AddNode<IModelChoiceActionItem>(ChangeViewModel);
            }
        }
    }

    public abstract class ModifyModelActionControllerBase : ViewController {
        private ActionModifyModelController _actionModifyModelController;

        protected override void OnActivated() {
            base.OnActivated();
            _actionModifyModelController = Frame.GetController<ActionModifyModelController>();
            _actionModifyModelController.ModifyModelAction.Execute += ModifyModelActionOnExecute;
        }

        public ActionModifyModelController ActionModifyModelController{
            get { return _actionModifyModelController; }
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            _actionModifyModelController = Frame.GetController<ActionModifyModelController>();
        }

        protected abstract void ModifyModelActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs e);

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _actionModifyModelController.ModifyModelAction.Execute -= ModifyModelActionOnExecute;
        }
    }
}