using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DictionaryStores;

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
            application.SetupComplete += OnSetupComplete;
        }

        void OnSetupComplete(object sender, EventArgs e) {
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled)
                new XpoModelDictionaryDifferenceStore(Application,  GetPath(), customModelDifferenceStoreEventArgs.ExtraDiffStores).Load((ModelApplicationBase) Application.Model);            
        }

        public abstract string GetPath();

    }
}