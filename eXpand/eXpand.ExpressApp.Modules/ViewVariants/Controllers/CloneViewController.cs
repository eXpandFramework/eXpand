using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ViewVariantsModule;
using eXpand.ExpressApp.ViewVariants.BasicObjects;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;

namespace eXpand.ExpressApp.ViewVariants.Controllers
{
    public interface IModelClassViewClonable:IModelClass {
        [Description("Determines if the clone action will be shown for the view")]
        [Category("eXpand.ViewVariants")]
        bool IsClonable { get; set; }
    }
    public interface IModelListViewViewClonable:IModelListView
    {
        [Category("eXpand.ViewVariants")]
        [Description("Determines if the clone action will be shown for the view")]
        [ModelValueCalculator("((IModelClassViewClonable)ModelClass)", "IsClonable")]
        bool IsClonable { get; set; }
    }

    public partial class CloneViewController : ViewController<ListView>, IModelExtender
    {
        public CloneViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewNesting = Nesting.Root;
        }

        private void cloneViewPopupWindowShowAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            ObjectSpace objectSpace = Application.CreateObjectSpace();
            e.View = Application.CreateDetailView(objectSpace, new ViewCloner(objectSpace.Session));
        }

        private void cloneViewPopupWindowShowAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ViewCloner viewCloner;
            var variantsNode = GetDefaultVariantsNode();
            var newVariantNode = GetNewVariantNode(variantsNode, e, out viewCloner);
            var clonedNode = (IModelListView)((ModelApplicationBase)View.Model.Application).CloneNodeFrom((ModelNode)View.Model);
            //setAttributes(clonedNode, viewCloner);
            Application.Model.Views.Add(clonedNode);

            var changeVariantController = Frame.GetController<ChangeVariantController>();
            SingleChoiceAction changeVariantAction = changeVariantController.ChangeVariantAction;
            if (changeVariantController.Active.ResultValue)
            {
                var choiceActionItem = new ChoiceActionItem(newVariantNode, newVariantNode.View);
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
            cloneViewPopupWindowShowAction.Active["IsClonable"] = ((IModelListViewViewClonable)View.Model).IsClonable;
        }

        private IModelVariant GetNewVariantNode(IModelVariants variantsNode, PopupWindowShowActionExecuteEventArgs e, out ViewCloner viewCloner) {
            var newVariantNode = variantsNode.AddNode<IModelVariant>("Variant");
            viewCloner = ((ViewCloner) e.PopupWindow.View.CurrentObject);
            newVariantNode.View.Id = viewCloner.Caption;
            setAttributes(newVariantNode, viewCloner);
            return newVariantNode;
        }

        private IModelVariants GetDefaultVariantsNode()
        {
            var variantsNode = View.Model as IModelVariants;
            if (variantsNode == null)
            {
                variantsNode = View.Model.AddNode<IModelVariants>();
                variantsNode.Current.Caption = "Default";
                var childNode = variantsNode.AddNode<IModelVariant>("Default");
                childNode.Caption = "Default";
                childNode.View.Id = View.Id;
            }

            return variantsNode;
        }

        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

        private void setAttributes(IModelVariant modelNode, ViewCloner viewCloner)
        {
            modelNode.Caption = viewCloner.Caption;
            modelNode.Id = viewCloner.Caption;
        }
    }
}