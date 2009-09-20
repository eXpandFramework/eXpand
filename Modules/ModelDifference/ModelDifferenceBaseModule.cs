using System;
using DevExpress.ExpressApp;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

namespace eXpand.ExpressApp.ModelDifference{
    public abstract class ModelDifferenceBaseModule<T> : ModuleBase where T : XpoModelDictionaryDifferenceStore
    {
        private string _connectionString;
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggingOn += OnLoggingOn;
            application.CreateCustomModelDifferenceStore += ApplicationOnCreateCustomModelDifferenceStore;
        }
        public PersistentApplication GetPersistentApplication(XafApplication application)
        {
            ObjectSpace objectSpace = application.CreateObjectSpace();
            PersistentApplication persistentApplication = new QueryPersistentApplication(objectSpace.Session).Find(application.GetType().FullName) ?? new PersistentApplication(objectSpace.Session);

            return persistentApplication;
        }

        private void OnLoggingOn(object sender, EventArgs args)
        {
            if ((bool) (!PersistentApplicationModelUpdated)){
                PersistentApplication application1 = GetPersistentApplication(Application);
                PersistentApplication persistentApplication = UpdatePersistentApplication(Application,
                                                                                          application1);
                ObjectSpace findObjectSpace = ObjectSpace.FindObjectSpace(persistentApplication);
                findObjectSpace.CommitChanges();
                findObjectSpace.Session.Disconnect();
                findObjectSpace.Dispose();
                PersistentApplicationModelUpdated = true;
            }
            Application.Model.CombineWith(getModelDiffs().Dictionary);
        }

        protected internal abstract bool? PersistentApplicationModelUpdated { get; set; }
            

        public PersistentApplication UpdatePersistentApplication(XafApplication application, PersistentApplication persistentApplication)
        {
            if (persistentApplication == null)
            {
                ObjectSpace objectSpace = application.CreateObjectSpace();
                persistentApplication = new PersistentApplication(objectSpace.Session);
            }
            persistentApplication.Model = application.Model;
            persistentApplication.UniqueName = application.GetType().FullName;
            if (string.IsNullOrEmpty(persistentApplication.Name))
                persistentApplication.Name = application.Title;
            return persistentApplication;
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