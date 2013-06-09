using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.ViewVariants {
    public interface IModelClassViewClonable {
        [Description("Determines if the clone action will be shown for the view")]
        [Category("eXpand.ViewVariants")]
        bool IsClonable { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassViewClonable), "ModelClass")]
    public interface IModelListViewViewClonable : IModelClassViewClonable {
    }

    public class ModifyVariantsController : ViewController<XpandListView>, IModelExtender {
        private const string IsClonable = "IsClonable";
        private const string Rename = "Rename";
        private const string Clone1 = "Clone";
        private const string Delete = "Delete";
        IModelView _rootView;
        readonly SingleChoiceAction _viewVariantsChoiceAction;

        public ModifyVariantsController() {
            _viewVariantsChoiceAction = new SingleChoiceAction(this, "ModifyViewVariants", PredefinedCategory.View);
            _viewVariantsChoiceAction.Execute += ViewVariantsChoiceActionOnExecute;
            _viewVariantsChoiceAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Clone1, Clone1));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Rename, Rename));
            _viewVariantsChoiceAction.Items.Add(new ChoiceActionItem(Delete, Delete));
            TargetViewNesting = Nesting.Root;
        }

        void ViewVariantsChoiceActionOnExecute(object sender, SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Clone1)) {
                CloneView(singleChoiceActionExecuteEventArgs);
            } else if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Rename)) {
                RenameView();
            } else if (ReferenceEquals(singleChoiceActionExecuteEventArgs.SelectedChoiceActionItem.Data, Delete)) {
                DeleteView();
            }
        }

        void CloneView(SingleChoiceActionExecuteEventArgs singleChoiceActionExecuteEventArgs) {
            var objectSpace = Application.CreateObjectSpace(typeof(ViewCloner));
            DetailView detailView = Application.CreateDetailView(objectSpace, objectSpace.CreateObject<ViewCloner>());
            detailView.ViewEditMode = ViewEditMode.Edit;
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "CreateViewCaption");
            singleChoiceActionExecuteEventArgs.ShowViewParameters.CreatedView = detailView;
            singleChoiceActionExecuteEventArgs.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            var dialogController = new DialogController();
            dialogController.Accepting += (o, args) => CloneView((ViewCloner)((DialogController)o).Frame.View.CurrentObject);
            singleChoiceActionExecuteEventArgs.ShowViewParameters.Controllers.Add(dialogController);
        }

        void RenameView() {
            var objectSpace = Application.CreateObjectSpace(typeof(ViewCloner));
            var viewCloner = objectSpace.CreateObject<ViewCloner>();
            viewCloner.Caption = Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Caption;
            var detailView = Application.CreateDetailView(objectSpace, viewCloner);
            detailView.ViewEditMode = ViewEditMode.Edit;
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.XpandViewVariants, "RenameViewToolTip");
            var parameters = new ShowViewParameters(detailView) { TargetWindow = TargetWindow.NewModalWindow };
            var controller = new DialogController();
            controller.AcceptAction.Execute += RenameViewActionOnExecute;
            parameters.Controllers.Add(controller);
            Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
        }
        private void RenameViewActionOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            var viewCloner = ((ViewCloner)args.CurrentObject);
            string id = View.Id;
            Frame.GetController<ModifyVariantsController>().CloneView(viewCloner);
            DeleteViewCore(id);
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            changeVariantController.RefreshVariantsAction();
            changeVariantController.ChangeVariantAction.SelectedItem = changeVariantController.ChangeVariantAction.FindItemByCaptionPath(viewCloner.Caption);
            changeVariantController.ChangeVariantAction.DoExecute(changeVariantController.ChangeVariantAction.SelectedItem);
            View.Refresh();
        }

        void DeleteView() {
            var variantInfo = (VariantInfo)Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Data;
            DeleteViewCore(variantInfo.ViewID);
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            changeVariantController.ChangeVariantAction.DoExecute(changeVariantController.ChangeVariantAction.SelectedItem);
            View.Refresh();
        }
        private void DeleteViewCore(string viewId) {
            IModelView modelView = RemoveVariantNode(viewId);
            Application.Model.Views[viewId].Remove();
            var changeVariantAction = Frame.GetController<ChangeVariantController>().ChangeVariantAction;
            changeVariantAction.Items.Remove(changeVariantAction.SelectedItem);
            changeVariantAction.SelectedItem = changeVariantAction.Items.SingleOrDefault(item => item.Caption == modelView.Id);
            View.SetModel(modelView);
        }


        private IModelView RemoveVariantNode(string viewId) {
            ViewShortcut viewShortcut = View.CreateShortcut();
            IModelView modelView = Application.Model.Views[viewShortcut.ViewId];
            IModelVariants modelVariants = ((IModelViewVariants)modelView).Variants;
            IModelVariant modelVariant = modelVariants.Single(variant => variant.View.Id == viewId);
            modelVariant.Remove();
            if (modelVariants.Count > 0) {
                modelVariants.Current = modelVariants[0];
                return modelVariants.Current.View;
            }
            return Application.Model.Views[viewShortcut.ViewId];
        }




        public void CloneView(ViewCloner viewCloner) {
            var newVariantNode = GetNewVariantNode(viewCloner);
            ActivateVariant(newVariantNode);
            View.SetModel(newVariantNode.View);
        }

        void ActivateVariant(IModelVariant newVariantNode) {
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            SingleChoiceAction changeVariantAction = changeVariantController.ChangeVariantAction;
            if (changeVariantController.Active.ResultValue) {
                var choiceActionItem = new ChoiceActionItem(newVariantNode.Caption, newVariantNode.View);
                changeVariantAction.Items.Add(choiceActionItem);
                changeVariantAction.SelectedItem = choiceActionItem;
            } else {
                changeVariantController.Frame.SetView(View);
                changeVariantAction.SelectedItem = (changeVariantAction.Items.Where(
                    item => item.Caption == newVariantNode.Caption)).Single();
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            _rootView = Application.Model.Views[View.CreateShortcut().ViewId];
        }
        protected override void OnViewControllersActivated() {
            base.OnViewControllersActivated();
            _viewVariantsChoiceAction.Active[IsClonable] = ((IModelListViewViewClonable)_rootView).IsClonable && Frame.GetController<ChangeVariantController>().ChangeVariantAction.Active;
        }

        private IModelVariant GetNewVariantNode(ViewCloner viewCloner) {
            IModelListView clonedView = GetClonedView(viewCloner.Caption);
            var modelViewVariants = ((IModelViewVariants)_rootView);
            IModelVariants modelVariants = modelViewVariants.Variants;
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current = newVariantNode;
            newVariantNode.Caption = viewCloner.Caption;
            newVariantNode.Id = viewCloner.Caption;
            newVariantNode.View = clonedView;
            return newVariantNode;
        }

        IModelListView GetClonedView(string caption) {
            var clonedView = ((IModelListView)((ModelNode)View.Model).Clone(caption));
            var modelViewVariants = ((IModelViewVariants)clonedView);
            modelViewVariants.Variants.Current = null;
            modelViewVariants.Variants.ClearNodes();
            return clonedView;
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

    }
}