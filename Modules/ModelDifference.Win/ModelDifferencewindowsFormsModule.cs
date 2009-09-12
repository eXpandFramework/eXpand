using System;
using DevExpress.ExpressApp;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public class ModelDifferenceBaseModule
    {
        
    }
    public sealed partial class ModelDifferenceWindowsFormsModule : ModuleBase
    {
        private string connectionString;
        
        public ModelDifferenceWindowsFormsModule()
        {
            InitializeComponent();
        }

        public override void Setup(XafApplication application){
            base.Setup(application);
            application.SetupComplete+=ApplicationOnSetupComplete;
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs args){

            var combiner = new DictionaryCombiner(Application.Model);
            combiner.AddAspects(getModelDiffs());
        }


        private DictionaryNode getModelDiffs(){
            using (var provider = new DevExpress.ExpressApp.ObjectSpaceProvider(new ConnectionStringDataStoreProvider(connectionString))){
                var xpoWinModelDictionaryDifferenceStore = new XpoWinModelDictionaryDifferenceStore(provider.CreateUpdatingSession(), Application,true);
                return xpoWinModelDictionaryDifferenceStore.LoadDifference(Application.Model.Schema).RootNode;
            }
        }


        private void ApplicationOnCreateCustomModelDifferenceStore(object sender,CreateCustomModelDifferenceStoreEventArgs args){
            args.Handled = true;
            connectionString = Application.ConnectionString;
            
        }
    }
}