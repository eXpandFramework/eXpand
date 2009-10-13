using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.ViewVariantsModule;

namespace eXpand.ExpressApp.ViewVariants.Controllers
{
    public partial class SynchronizeActionWithViewController : ViewController
    {
        public SynchronizeActionWithViewController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += OnCustomShowNavigationItem;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            if (View is ListView)
            {
                SingleChoiceAction changeVariantAction = Frame.GetController<ChangeVariantController>().ChangeVariantAction;
                changeVariantAction.SelectedItem = changeVariantAction.Items.Find(View.Id);
            }
            
        }

        private void OnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs args)
        {
            string viewId = args.ActionArguments.SelectedChoiceActionItem.Info.GetAttributeValue("ViewID");
            var viewInfoNodeWrapper = (ListViewInfoNodeWrapper) new ApplicationNodeWrapper(Application.Info).Views.FindViewById(viewId);
            var node = viewInfoNodeWrapper.Node.FindChildNode("Variants");
            if (node != null)
            {
                var cuurentViewId = node.GetAttribute("Current").Value;
                if (!string.IsNullOrEmpty(cuurentViewId)&&cuurentViewId!="Default")
                    ((ViewShortcut)args.ActionArguments.SelectedChoiceActionItem.Data).ViewId = cuurentViewId;
            }
        }
    }
}
