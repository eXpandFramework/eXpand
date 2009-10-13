using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.ViewVariantsModule.Win.Controllers
{
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
                repositoryItem.Buttons.AddRange(new[] {new EditorButton(ButtonPredefines.Delete),});
                repositoryItem.ButtonClick += RepositoryItem_OnButtonClick;
            }

        }



        private void RepositoryItem_OnButtonClick(object sender, ButtonPressedEventArgs e)
        {
            
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                if (MessageBox.Show("Delete current view?", null,MessageBoxButtons.YesNo)==DialogResult.Yes)
                {
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
            }
            
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