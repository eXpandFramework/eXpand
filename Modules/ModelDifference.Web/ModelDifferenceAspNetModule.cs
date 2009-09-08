using System.ComponentModel;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class ModelDifferenceAspNetModule : ModuleBase
    {
        public ModelDifferenceAspNetModule()
        {
            InitializeComponent();
        }


        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }

        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            args.Handled = true;
            using (var provider = new ObjectSpaceProvider(new ConnectionStringDataStoreProvider(Application.ConnectionString)))
            {
                args.Store = new XpoWebModelDictionaryDifferenceStore(provider.CreateUpdatingSession(), Application);
            }


        }
    }
}