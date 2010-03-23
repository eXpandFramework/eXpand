using System;
using DevExpress.ExpressApp;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using DevExpress.ExpressApp.Updating;

namespace eXpand.ExpressApp.ModelDifference{
    public abstract class ModelDifferenceBaseModule<T> : ModuleBase where T : XpoModelDictionaryDifferenceStore
    {
        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        protected virtual void InvokeCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e) {
            EventHandler<CreateCustomModelDifferenceStoreEventArgs> handler = CreateCustomModelDifferenceStore;
            if (handler != null) handler(this, e);
        }     

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggingOn += OnLoggingOn;
            application.SetupComplete += OnSetupComplete;
        }

        public PersistentApplication GetPersistentApplication(XafApplication application)
        {
            var work = application.ObjectSpaceProvider.CreateObjectSpace().Session;
            PersistentApplication persistentApplication = new QueryPersistentApplication(work).Find(application.GetType().FullName) ?? new PersistentApplication(work);
            return persistentApplication;
        }

        private void OnSetupComplete(object sender, EventArgs args){
            var dbUpdater = new DatabaseUpdater(Application.ObjectSpaceProvider, Application.Modules, Application.ApplicationName);
            CompatibilityError compatibilityError = dbUpdater.CheckCompatibility();
            if ((bool)(!PersistentApplicationModelUpdated) && compatibilityError != null && compatibilityError is CompatibilityDatabaseIsOldError){
                PersistentApplication persistentApplication = UpdatePersistentApplication(Application);
                ((UnitOfWork)persistentApplication.Session).CommitChanges();
                PersistentApplicationModelUpdated = true;
            }
        }

        private void OnLoggingOn(object sender, EventArgs args)
        {
            Dictionary dictionary = getModelDiffs().Dictionary;
            Application.Model.CombineWith(dictionary);
        }

        protected internal abstract bool? PersistentApplicationModelUpdated { get; set; }

        public PersistentApplication UpdatePersistentApplication(XafApplication application)
        {
            PersistentApplication persistentApplication = GetPersistentApplication(Application);
            persistentApplication.Model = application.Model;
            persistentApplication.UniqueName = application.GetType().FullName;
            if (string.IsNullOrEmpty(persistentApplication.Name))
                persistentApplication.Name = application.Title;
            return persistentApplication;
        }

        private DictionaryNode getModelDiffs()
        {
            var args = new CreateCustomModelDifferenceStoreEventArgs();
            InvokeCreateCustomModelDifferenceStore(args);
            if (!args.Handled) {
                return new XpoModelDictionaryDifferenceStoreFactory<T>().Create(Application, true).LoadDifference(
                        Application.Model.Schema).RootNode;
            }
            return args.Store.LoadDifference(Application.Model.Schema).RootNode;
        }


    }
}