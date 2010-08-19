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
    public interface IModelClassViewClonable{
        [Description("Determines if the clone action will be shown for the view")]
        [Category("eXpand.ViewVariants")]
        bool IsClonable { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelClassViewClonable), "ModelClass")]
    public interface IModelListViewViewClonable : IModelClassViewClonable
    {
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
            
            var newVariantNode = GetNewVariantNode((ViewCloner) e.PopupWindow.View.CurrentObject);
            ActivateVariant(newVariantNode);
            View.SetInfo(newVariantNode.View);
        }

        void ActivateVariant(IModelVariant newVariantNode) {
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
                                                    where item.Caption == newVariantNode.Caption
                                                    select item).Single();
            }
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            cloneViewPopupWindowShowAction.Active["IsClonable"] = ((IModelListViewViewClonable)View.Model).IsClonable;
        }

        private IModelVariant GetNewVariantNode(ViewCloner viewCloner) {
            var modelViewVariants = ((IModelViewVariants)View.Model);
            IModelVariants modelVariants = modelViewVariants.Variants;
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current=newVariantNode;
            newVariantNode.Caption = viewCloner.Caption;
            newVariantNode.Id = viewCloner.Caption;
            IModelListView clonedView = GetClonedView(viewCloner.Caption);
            newVariantNode.View = clonedView;
            return newVariantNode;
        }

        IModelListView GetClonedView(string caption) {
            var clonedView = (IModelListView)((ModelApplicationBase)View.Model.Application).CloneNodeFrom((ModelNode)View.Model,caption);
            Application.Model.Views.Add(clonedView);
            return clonedView;
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

    }
}