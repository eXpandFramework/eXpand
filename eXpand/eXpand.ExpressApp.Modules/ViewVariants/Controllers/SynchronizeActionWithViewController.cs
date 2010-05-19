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
            string viewId = (args.ActionArguments.SelectedChoiceActionItem.Data as ViewShortcut).ViewId;

            var variants = Application.Model.Views[viewId] as IModelViewVariants;
            if (variants != null)
            {
                var variantName = variants.Variants.Current.Caption;
                if (!string.IsNullOrEmpty(variantName) && variantName != "Default")
                    ((ViewShortcut)args.ActionArguments.SelectedChoiceActionItem.Data).ViewId = variants.Variants[variantName].View.Id;
            }
        }
    }
}
