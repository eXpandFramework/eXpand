using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public sealed partial class ModelDifferenceWindowsFormsModule : ModuleBase
    {
        public ModelDifferenceWindowsFormsModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }

        private void ApplicationOnCreateCustomModelDifferenceStore(object sender,
                                                                   CreateCustomModelDifferenceStoreEventArgs args){
            args.Handled = true;
            using (
                var provider =
                    new DevExpress.ExpressApp.ObjectSpaceProvider(
                        new ConnectionStringDataStoreProvider(Application.ConnectionString))){
                args.Store = new XpoWinModelDictionaryDifferenceStore(provider.CreateUpdatingSession(), Application);
            }
        }
    }
}