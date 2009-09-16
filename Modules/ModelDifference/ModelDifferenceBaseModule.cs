using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference{
    public class ModelDifferenceBaseModule<T> : ModuleBase where T : XpoModelDictionaryDifferenceStore
    {
        private string _connectionString;
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggingOn += OnLoggedOn;
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }

        private void OnLoggedOn(object sender, EventArgs args)
        {
            Application.Model.CombineWith(getModelDiffs().Dictionary);
//            var combiner = new DictionaryCombiner(Application.Model);
//            combiner.AddAspects(getModelDiffs());
        }
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        private DictionaryNode getModelDiffs()
        {
            using (var provider =new DevExpress.ExpressApp.ObjectSpaceProvider(new ConnectionStringDataStoreProvider(_connectionString))){
                return new XpoModelDictionaryDifferenceStoreFactory<T>().Create(provider.CreateUpdatingSession(),Application, true).LoadDifference(Application.Model.Schema).RootNode;
            }
        }


        private void ApplicationOnCreateCustomModelDifferenceStore(object sender, CreateCustomModelDifferenceStoreEventArgs args)
        {
            args.Handled = true;
            if (_connectionString== null)
                _connectionString = Application.ConnectionString;
        }
    }
}