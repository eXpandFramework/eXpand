using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference.Win{
    public class ModelDifferenceBaseModule<T> : ModuleBase where T : XpoModelDictionaryDifferenceStore
    {
        private string connectionString;
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.SetupComplete += ApplicationOnSetupComplete;
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }

        private void ApplicationOnSetupComplete(object sender, EventArgs args)
        {

            var combiner = new DictionaryCombiner(Application.Model);
            combiner.AddAspects(getModelDiffs());
        }


        private DictionaryNode getModelDiffs()
        {
            using (var provider =new DevExpress.ExpressApp.ObjectSpaceProvider(new ConnectionStringDataStoreProvider(connectionString))){
                return new XpoModelDictionaryDifferenceStoreFactory<T>().Create(provider.CreateUpdatingSession(),Application, true).LoadDifference(Application.Model.Schema).RootNode;
            }
        }


        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            args.Handled = true;
            connectionString = Application.ConnectionString;

        }
    }
}