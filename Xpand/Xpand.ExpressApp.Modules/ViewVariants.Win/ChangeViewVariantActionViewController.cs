using System.Linq;
using DevExpress.ExpressApp.Utils;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.ViewVariants.Win {
    public class ChangeViewVariantActionViewController : ViewController<XpandListView>
    {
        
        protected override void OnAfterConstruction()
        {
            base.OnAfterConstruction();
            BarActionItemsFactory.CustomizeActionControl += BarActionItemsFactoryOnCustomizeActionControl;
        }

        private void BarActionItemsFactoryOnCustomizeActionControl(object sender, CustomizeActionControlEventArgs args)
        {
            if (Frame != null && args.Action.Id == Frame.GetController<ChangeVariantController>().ChangeVariantAction.Id &&
                Frame.Template is IBarManagerHolder) {
                var repositoryItem = (RepositoryItemImageComboBox) ((BarEditItem) args.ActionControl.Control).Edit;
                var renameButton = new EditorButton(ButtonPredefines.Ellipsis) { ToolTip = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.EXpandViewVariants, "RenameViewToolTip") };
                var deleteButton = new EditorButton(ButtonPredefines.Delete) { ToolTip = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.EXpandViewVariants, "RenameViewToolTip") };
                repositoryItem.Buttons.AddRange(new[] { renameButton,deleteButton });
                repositoryItem.ButtonClick += RepositoryItem_OnButtonClick;
            }

        }

        private void RepositoryItem_OnButtonClick(object sender, ButtonPressedEventArgs e){
            if (e.Button.Kind == ButtonPredefines.Delete){
                DeleteView();
            }
            else if (e.Button.Kind==ButtonPredefines.Ellipsis) {
                RenameView();
            }
        }

        void RenameView() {
            var objectSpace = Application.CreateObjectSpace();
            var viewCloner = new ViewCloner(objectSpace.Session){Caption = Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Caption};
            var detailView = Application.CreateDetailView(objectSpace, viewCloner);
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.EXpandViewVariants, "RenameViewToolTip");
            var parameters = new ShowViewParameters(detailView) {TargetWindow = TargetWindow.NewModalWindow};
            var controller = new DialogController();
            controller.AcceptAction.Execute+=RenameViewActionOnExecute;
            parameters.Controllers.Add(controller);
            Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
        }

        void DeleteView() {
            if (MessageBox.Show(CaptionHelper.GetLocalizedText(XpandViewVariantsModule.EXpandViewVariants, "DeleteViewConfirmation"), null, MessageBoxButtons.YesNo) == DialogResult.Yes){
                var variantInfo = (VariantInfo) Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Data;
                DeleteViewCore(variantInfo.ViewID);
                var changeVariantController = Frame.GetController<ChangeVariantController>();
                changeVariantController.ChangeVariantAction.DoExecute(changeVariantController.ChangeVariantAction.SelectedItem);
                View.Refresh();
            }
        }

        private void RenameViewActionOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            var viewCloner = ((ViewCloner)args.CurrentObject);
            string id = View.Id;
            Frame.GetController<CloneViewController>().Clone(viewCloner);
            DeleteViewCore(id);
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            changeVariantController.RefreshVariantsAction();
            changeVariantController.ChangeVariantAction.SelectedItem = changeVariantController.ChangeVariantAction.FindItemByCaptionPath(viewCloner.Caption);
            changeVariantController.ChangeVariantAction.DoExecute(changeVariantController.ChangeVariantAction.SelectedItem);
            View.Refresh();
        }


        private void DeleteViewCore(string viewId) {
            IModelView modelView = RemoveVariantNode(viewId);
            Application.Model.Views.Remove(Application.Model.Views[viewId]);
            var changeVariantAction = Frame.GetController<ChangeVariantController>().ChangeVariantAction;
            changeVariantAction.Items.Remove(changeVariantAction.SelectedItem);
            changeVariantAction.SelectedItem =changeVariantAction.Items.Where(item => item.Caption == modelView.Id).SingleOrDefault();
            View.SetInfo(modelView);
        }


        private IModelView RemoveVariantNode(string viewId)
        {
            ViewShortcut viewShortcut = View.CreateShortcut();
            IModelView modelView = Application.Model.Views[viewShortcut.ViewId];
            IModelVariants modelVariants = ((IModelViewVariants) modelView).Variants;
            IModelVariant modelVariant = modelVariants.Where(variant => variant.View.Id==viewId).Single();
            modelVariants.Remove(modelVariant);
            if (modelVariants.Count > 0) {
                modelVariants.Current = modelVariants[0];
                return modelVariants.Current.View;
            }
            return Application.Model.Views[viewShortcut.ViewId];
        }
    }
}