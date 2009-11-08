using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.ViewVariants.BasicObjects;

namespace eXpand.ExpressApp.ViewVariants.Win.Controllers {
    public partial class ChangeViewVariantActionViewController : BaseViewController
    {
        public ChangeViewVariantActionViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnAfterConstruction()
        {
            base.OnAfterConstruction();
            BarActionItemsFactory<ActionBase>.CustomizeActionControl += BarActionItemsFactoryOnCustomizeActionControl;
            
        }

        private void BarActionItemsFactoryOnCustomizeActionControl(object sender, CustomizeActionControlEventArgs<ActionBase> args)
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
                if (MessageBox.Show("Delete current view?", null,MessageBoxButtons.YesNo)==DialogResult.Yes) {
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
            var listViewInfoNodeWrapper = new ListViewInfoNodeWrapper(View.Info);
            var newCaption = ((ViewCloner)args.CurrentObject).Caption;
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            var changeVariantAction = changeVariantController.ChangeVariantAction;
            setCurrentVariant(changeVariantAction.SelectedItem.Caption,newCaption);
            listViewInfoNodeWrapper.Id=newCaption;
            listViewInfoNodeWrapper.Caption = listViewInfoNodeWrapper.Id;
            changeVariantAction.SelectedItem.Caption =listViewInfoNodeWrapper.Caption;
            changeVariantAction.SelectedItem.Data = listViewInfoNodeWrapper.Id;
            View.SetInfo(View.Info);
            View.Refresh();
            var showNavigationItemController=Frame.GetController<ShowNavigationItemController>();
            ((ViewShortcut) showNavigationItemController.ShowNavigationItemAction.SelectedItem.Data).ViewId =listViewInfoNodeWrapper.Id;

        }

        private void setCurrentVariant(string oldId, string newId) {
            var wrapper = new ApplicationNodeWrapper(Application.Info);
            var variantNodes = wrapper.Views.Items.OfType<ListViewInfoNodeWrapper>().Where(nodeWrapper => nodeWrapper.Node.FindChildNode("Variants") != null).Select(nodeWrapper => nodeWrapper.Node.FindChildNode("Variants")).SelectMany(node => node.ChildNodes).ToList();
            foreach (DictionaryNode node in variantNodes) {
                if (node.KeyAttribute.Value == oldId) {
                    node.Parent.SetAttribute("Current", newId);
                    node.SetAttribute("ID", newId);
                    node.SetAttribute("ViewID", newId);
                    node.SetAttribute("Caption", newId);
                }
            }
        }

        private void deleteView(object sender) {
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            if (changeVariantController.ChangeVariantAction.Items.Count == 1) {
                MessageBox.Show("You cannot delete all views");
                return;
            }
            var wrapper = new ApplicationNodeWrapper(Application.Info);
            var choiceActionItem = ((ChoiceActionItem)((ComboBoxEdit)sender).EditValue);
            var id = choiceActionItem.Data.ToString();
            var childNode = wrapper.Views.FindViewById(id).Node;
            removeVariantNodeFromViews(choiceActionItem.Info.KeyAttribute.Value, wrapper);
            wrapper.Views.Node.RemoveChildNode(childNode);
            ListViewInfoNodeWrapper node1 = GetCurrentVariant(changeVariantController, wrapper);
            View.SetInfo(node1.Node);
        }

        private ListViewInfoNodeWrapper GetCurrentVariant(ChangeVariantController changeVariantController, ApplicationNodeWrapper wrapper) {
            var node1 = new ListViewInfoNodeWrapper(wrapper.Views.FindViewById(Application.FindListViewId(View.ObjectTypeInfo.Type)).Node);
            changeVariantController.ChangeVariantAction.Items.Remove(
                changeVariantController.ChangeVariantAction.SelectedItem);
            changeVariantController.ChangeVariantAction.SelectedItem = (from item in changeVariantController.ChangeVariantAction.Items
                                                                        where item.Caption == node1.Node.FindChildNode("Variants").ChildNodes[0].KeyAttribute.Value
                                                                        select item).Single();
            return node1;
        }

        private void removeVariantNodeFromViews(string id, ApplicationNodeWrapper wrapper) {
            var variantNodes = wrapper.Views.Items.OfType<ListViewInfoNodeWrapper>().Where(nodeWrapper => nodeWrapper.Node.FindChildNode("Variants")!=null).Select(nodeWrapper => nodeWrapper.Node.FindChildNode("Variants")).SelectMany(node => node.ChildNodes).ToList();
            foreach (var node in variantNodes) {
                if (node.KeyAttribute.Value==id) {
                    node.Parent.SetAttribute("Current",node.Parent.ChildNodes.Where(dictionaryNode => dictionaryNode.KeyAttribute.Value!=id).ToList()[0].KeyAttribute.Value);
                    node.Parent.RemoveChildNode(node);    
                }
            }
        }
    }
}