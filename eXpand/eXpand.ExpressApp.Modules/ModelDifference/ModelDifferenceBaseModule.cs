using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;
using System.Reflection;

namespace eXpand.ExpressApp.ModelDifference{
    public abstract class ModelDifferenceBaseModule : ModuleBase 
    {
        protected internal abstract bool? PersistentApplicationModelUpdated { get; set; }
        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        public void OnCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e)
        {
            EventHandler<CreateCustomModelDifferenceStoreEventArgs> handler = CreateCustomModelDifferenceStore;
            if (handler != null) handler(this, e);
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggingOn += OnLoggingOn;
            application.SetupComplete += OnSetupComplete;
        }

        public abstract string GetPath();
        private void OnSetupComplete(object sender, EventArgs args)
        {
            var dbUpdater = new DatabaseUpdater(Application.ObjectSpaceProvider, Application.Modules, Application.ApplicationName);
            CompatibilityError compatibilityError = dbUpdater.CheckCompatibility();
            if ((bool)(!PersistentApplicationModelUpdated) && compatibilityError != null && compatibilityError is CompatibilityDatabaseIsOldError)
            {
                PersistentApplication persistentApplication = UpdatePersistentApplication(Application);
                ((UnitOfWork)persistentApplication.Session).CommitChanges();
                PersistentApplicationModelUpdated = true;
            }
        }

        private PersistentApplication UpdatePersistentApplication(XafApplication application)
        {
            PersistentApplication persistentApplication = GetPersistentApplication(Application);
            persistentApplication.UniqueName = application.GetType().FullName;
            persistentApplication.ExecutableName = Assembly.GetAssembly(application.GetType()).ManifestModule.Name;
            if (string.IsNullOrEmpty(persistentApplication.Name))
                persistentApplication.Name = application.Title;
            return persistentApplication;
        }

        private PersistentApplication GetPersistentApplication(XafApplication application)
        {
            var work = application.ObjectSpaceProvider.CreateObjectSpace().Session;
            PersistentApplication persistentApplication = new QueryPersistentApplication(work).Find(application.GetType().FullName) ?? new PersistentApplication(work);
            return persistentApplication;
        }

        private void OnLoggingOn(object sender, EventArgs args)
        {
            var createCustomModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(createCustomModelDifferenceStoreEventArgs);
            if (!createCustomModelDifferenceStoreEventArgs.Handled)
                GetApplicationModelDiffs(createCustomModelDifferenceStoreEventArgs.ExtraDiffStores);

        }

        private void GetApplicationModelDiffs(List<ModelApplicationFromStreamStoreBase> extraDiffStores)
        {
            new XpoModelDictionaryDifferenceStore(Application, true, GetPath(), extraDiffStores).Load((ModelApplicationBase)Application.Model);
        }
    }
}