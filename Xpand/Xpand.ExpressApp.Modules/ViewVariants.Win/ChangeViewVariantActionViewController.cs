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
        private const string EXpandViewVariants = "eXpand.ViewVariants";
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
                repositoryItem.Buttons.AddRange(new[] { new EditorButton(ButtonPredefines.Ellipsis),new EditorButton(ButtonPredefines.Delete) });
                repositoryItem.ButtonClick += RepositoryItem_OnButtonClick;
            }

        }

        private void RepositoryItem_OnButtonClick(object sender, ButtonPressedEventArgs e)
        {
            
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (MessageBox.Show(CaptionHelper.GetLocalizedText(EXpandViewVariants, "DeleteViewConfirmation"), null, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var variantInfo = (VariantInfo) Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Data;
                    DeleteView(variantInfo.ViewID);
                }
            }
            else if (e.Button.Kind==ButtonPredefines.Ellipsis) {
                
                var objectSpace = Application.CreateObjectSpace();
                var viewCloner = new ViewCloner(objectSpace.Session){Caption = Frame.GetController<ChangeVariantController>().ChangeVariantAction.SelectedItem.Caption};
                var detailView = Application.CreateDetailView(objectSpace, viewCloner);
                var parameters = new ShowViewParameters(detailView) {TargetWindow = TargetWindow.NewModalWindow};
                var controller = new DialogController();
                controller.AcceptAction.Execute+=EditViewActionOnExecute;
                parameters.Controllers.Add(controller);
                Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(null, null));
            }
        }

        private void EditViewActionOnExecute(object sender, SimpleActionExecuteEventArgs args) {
            var viewCloner = ((ViewCloner)args.CurrentObject);
            string id = View.Id;
            Frame.GetController<CloneViewController>().Clone(viewCloner);
            DeleteView(id);
            Frame.GetController<ChangeVariantController>().RefreshVariantsAction();
        }


        private void DeleteView(string viewId) {
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