using System.ComponentModel;
using System.Data;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.DictionaryDifferenceStore.Web
{
    [ToolboxItemFilter("Xaf.Platform.Web")]
    public sealed partial class DictionaryDifferenceStoreAspNetModule : ModuleBase
    {
        private IObjectSpaceProvider objectSpaceProvider;

        public DictionaryDifferenceStoreAspNetModule()
        {
            InitializeComponent();
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CreateCustomObjectSpaceProvider += ApplicationOnCreateCustomObjectSpaceProvider;
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }
        private void ApplicationOnCreateCustomObjectSpaceProvider(object sender, CreateCustomObjectSpaceProviderEventArgs args)
        {
            objectSpaceProvider = args.ObjectSpaceProvider;
        }

        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            if (objectSpaceProvider == null)
                throw new NoNullAllowedException("Custom objectSpaceProvider");
            args.Handled = true;
            args.Store =
                new XpoWebModelDictionaryDifferenceStore(objectSpaceProvider.CreateUpdatingSession(),
                                                         Application);
        }

    }
}