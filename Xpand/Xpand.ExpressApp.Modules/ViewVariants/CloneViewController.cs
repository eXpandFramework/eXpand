using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo;

namespace Xpand.ExpressApp.ViewVariants
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

    public partial class CloneViewController : ViewController<XpandListView>, IModelExtender
    {
        IModelView _rootView;

        public CloneViewController()
        {
            InitializeComponent();
            RegisterActions(components);
            TargetViewNesting = Nesting.Root;
        }

        private void cloneViewPopupWindowShowAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();
            DetailView detailView = Application.CreateDetailView(objectSpace, objectSpace.CreateObject<ViewCloner>());
            detailView.Caption = CaptionHelper.GetLocalizedText(XpandViewVariantsModule.EXpandViewVariants,"CreateViewCaption");
            e.View = detailView;
        }

        private void cloneViewPopupWindowShowAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            var viewCloner = (ViewCloner) e.PopupWindow.View.CurrentObject;
            Clone(viewCloner);
        }

        public void Clone(ViewCloner viewCloner) {
            var newVariantNode = GetNewVariantNode(viewCloner);
            ActivateVariant(newVariantNode);
            View.SetInfo(newVariantNode.View);
        }

        void ActivateVariant(IModelVariant newVariantNode) {
            var changeVariantController = Frame.GetController<ChangeVariantController>();
            SingleChoiceAction changeVariantAction = changeVariantController.ChangeVariantAction;
            if (changeVariantController.Active.ResultValue)
            {
                var choiceActionItem = new ChoiceActionItem(newVariantNode.Caption, newVariantNode.View);
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
            _rootView = Application.Model.Views[View.CreateShortcut().ViewId];
            cloneViewPopupWindowShowAction.Active["IsClonable"] = ((IModelListViewViewClonable)_rootView).IsClonable;
        }

        private IModelVariant GetNewVariantNode(ViewCloner viewCloner) {
            IModelListView clonedView = GetClonedView(viewCloner.Caption);
            var modelViewVariants = ((IModelViewVariants)_rootView);
            IModelVariants modelVariants = modelViewVariants.Variants;
            var newVariantNode = modelVariants.AddNode<IModelVariant>();
            modelVariants.Current=newVariantNode;
            newVariantNode.Caption = viewCloner.Caption;
            newVariantNode.Id = viewCloner.Caption;
            newVariantNode.View = clonedView;
            return newVariantNode;
        }

        IModelListView GetClonedView(string caption) {
            var clonedView = (IModelListView)(((ModelNode)View.Model.Parent)).CloneNodeFrom((ModelNode)View.Model, caption);
            var modelViewVariants = ((IModelViewVariants) clonedView);
            modelViewVariants.Variants.Current = null;            
            modelViewVariants.Variants.Clear();
            return clonedView;
        }


        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders)
        {
            extenders.Add<IModelListView, IModelListViewViewClonable>();
            extenders.Add<IModelClass, IModelClassViewClonable>();
        }

    }
}