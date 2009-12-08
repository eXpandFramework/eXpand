using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ViewVariantsModule;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.SystemModule;
using eXpand.ExpressApp.ViewVariants.BasicObjects;

namespace eXpand.ExpressApp.ViewVariants.Controllers
{
    public partial class CloneViewController : BaseViewController
    {
        public const string IsCloneAbleAttributeName = "IsCloneAble";
        public CloneViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Root;
        }



        private void cloneViewPopupWindowShowAction_CustomizePopupWindowParams(object sender,
                                                                               CustomizePopupWindowParamsEventArgs e)
        {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, new ViewCloner(objectSpace.Session));
        }

        private void cloneViewPopupWindowShowAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DictionaryNode variantsNode = GetDefaultVariantsNode();
            ViewCloner viewCloner;
            DictionaryNode newVariantNode = GetNewVariantNode(variantsNode, e, out viewCloner);

            DictionaryNode clonedNode = View.Info.Clone();
            setAttributes(clonedNode, viewCloner);
            Application.Model.RootNode.FindChildNode("Views").AddChildNode(clonedNode);

            var changeVariantController = Frame.GetController<ChangeVariantController>();
            SingleChoiceAction changeVariantAction = changeVariantController.ChangeVariantAction;
            if (changeVariantController.Active.ResultValue)
            {
                var choiceActionItem = new ChoiceActionItem(newVariantNode,
                                                            newVariantNode.GetAttributeValue("ViewID"));
                changeVariantAction.Items.Add(choiceActionItem);
                changeVariantAction.SelectedItem=choiceActionItem;
            }
            else
            {
                changeVariantController.Frame.SetView(View);
                changeVariantAction.SelectedItem = (from item in changeVariantAction.Items
                                                    where item.Caption == viewCloner.Caption
                                                    select item).Single();
            }
            View.SetInfo(clonedNode);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            cloneViewPopupWindowShowAction.Active[IsCloneAbleAttributeName] =View.Info.GetAttributeBoolValue(IsCloneAbleAttributeName);
        }
        private DictionaryNode GetNewVariantNode(DictionaryNode variantsNode, PopupWindowShowActionExecuteEventArgs e, out ViewCloner viewCloner) {
            DictionaryNode newVariantNode = variantsNode.AddChildNode("Variant");
            viewCloner = ((ViewCloner) e.PopupWindow.View.CurrentObject);
            newVariantNode.SetAttribute("ViewID", viewCloner.Caption);
            setAttributes(newVariantNode, viewCloner);
            return newVariantNode;
        }

        private DictionaryNode GetDefaultVariantsNode() {
            DictionaryNode variantsNode = View.Info.FindChildNode("Variants");
            if (variantsNode == null)
            {
                variantsNode = View.Info.AddChildNode("Variants");
                variantsNode.AddAttribute("Current", "Default");
                DictionaryNode childNode = variantsNode.AddChildNode("Default");
                childNode.AddAttribute("Caption", "Default");
                childNode.AddAttribute("ID", "Default");
                childNode.AddAttribute("ViewID", View.Id);
            }
            return variantsNode;
        }
        public override Schema GetSchema()
        {
            const string s = @"<Attribute Name=""" + IsCloneAbleAttributeName + @""" Choice=""True,False""/>";
            var helper=new SchemaHelper();
            var schema = new Schema(helper.Inject(s, ModelElement.ListView));
            return schema;
        }
        private void setAttributes(DictionaryNode dictionaryNode, ViewCloner viewCloner)
        {
            dictionaryNode.SetAttribute("Caption", viewCloner.Caption);
            dictionaryNode.SetAttribute("ID", viewCloner.Caption);
        }
    }
}