using System.Linq;
using DevExpress.ExpressApp.Utils;
using Windows = System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using eXpand.ExpressApp.ViewVariants.BasicObjects;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.ViewVariants.Win.Controllers {
    public class ChangeViewVariantActionViewController : ViewController<ListView>
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
                repositoryItem.Buttons.AddRange(new[] { new EditorButton(ButtonPredefines.Ellipsis),new EditorButton(ButtonPredefines.Delete) });
                repositoryItem.ButtonClick += RepositoryItem_OnButtonClick;
            }

        }

        private void RepositoryItem_OnButtonClick(object sender, ButtonPressedEventArgs e)
        {
            
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (Windows.MessageBox.Show(CaptionHelper.GetLocalizedText("eXpand.ViewVariants", "DeleteViewConfirmation"), null, Windows.MessageBoxButtons.YesNo) == Windows.DialogResult.Yes)
                {
                    deleteView(sender);
                    return;
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
            var newCaption = ((ViewCloner)args.CurrentObject).Caption;
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            var changeVariantAction = changeVariantController.ChangeVariantAction;
            setCurrentVariant(changeVariantAction.SelectedItem.Caption,newCaption);
            changeVariantAction.SelectedItem.Caption = newCaption;
            changeVariantAction.SelectedItem.Data = newCaption;
            View.SetInfo(View.Model);
            View.Refresh();
            var showNavigationItemController=Frame.GetController<ShowNavigationItemController>();
            ((ViewShortcut)showNavigationItemController.ShowNavigationItemAction.SelectedItem.Data).ViewId = newCaption;

        }

        private void setCurrentVariant(string oldId, string newId) {
            var variantNodes = Application.Model.Views.OfType<IModelListView>().Where(nodeWrapper => ((IModelViewVariants)nodeWrapper).Variants != null).Select(nodeWrapper => ((IModelViewVariants)nodeWrapper).Variants).SelectMany(node => node.ToList()).ToList();
            foreach (var node in variantNodes) {
                if (node.Id == oldId) {
                    ((IModelVariants)node.Parent).Current = ((IModelVariants)node.Parent)[newId];
                    node.Id = newId;
                    node.View.Id = newId;
                    node.Caption = newId;
                }
            }
        }

        private void deleteView(object sender) {
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            if (changeVariantController.ChangeVariantAction.Items.Count == 1) {
                Windows.MessageBox.Show("You cannot delete all views");
                return;
            }
            
            var choiceActionItem = ((ChoiceActionItem)((ComboBoxEdit)sender).EditValue);
            var id = choiceActionItem.Data.ToString();
            
            removeVariantNodeFromViews(choiceActionItem.Id);
            Application.Model.Views.Remove(Application.Model.Views[id]);
            var view = GetCurrentVariant(changeVariantController);
            View.SetInfo(view);
        }

        private IModelView GetCurrentVariant(ChangeVariantController changeVariantController) {
            var view = Application.Model.Views[Application.FindListViewId(View.ObjectTypeInfo.Type)];
            changeVariantController.ChangeVariantAction.Items.Remove(
                changeVariantController.ChangeVariantAction.SelectedItem);
            changeVariantController.ChangeVariantAction.SelectedItem = (from item in changeVariantController.ChangeVariantAction.Items
                                                                        where item.Caption == ((IModelViewVariants)view).Variants[0].Id
                                                                        select item).Single();
            return view;
        }

        private void removeVariantNodeFromViews(string id) {
            var variantNodes = Application.Model.Views.OfType<IModelListView>().Where(nodeWrapper => ((IModelViewVariants)nodeWrapper).Variants != null).Select(nodeWrapper => ((IModelViewVariants)nodeWrapper).Variants).SelectMany(node => node.ToList()).ToList();
            foreach (var node in variantNodes) {
                if (node.Id == id) {
                    ((IModelVariants)node.Parent).Current = ((IModelVariants)node.Parent).Where(dictionaryNode => dictionaryNode.Id != id).ToList()[0];
                    ((IModelVariants)node.Parent).Remove(node);    
                }
            }
        }
    }
}