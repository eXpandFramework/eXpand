using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference {
    public abstract class ModelDifferenceBaseModule : XpandModuleBase {
        protected abstract bool? ModelsLoaded { get; set; }
        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        public void OnCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e) {
            EventHandler<CreateCustomModelDifferenceStoreEventArgs> handler = CreateCustomModelDifferenceStore;
            if (handler != null) handler(this, e);
        }
        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) {
                Application.LoggingOn += (sender, args) => {
                    if (ModelsLoaded.HasValue)
                        LoadModels(!ModelsLoaded.Value);
                    ModelsLoaded = true;
                };
            }
        }



        public void LoadModels(bool loadResources) {
            ((ModelApplicationBase)Application.Model).ReInitLayers();
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled)
                new XpoModelDictionaryDifferenceStore(Application, GetPath(), customModelDifferenceStoreEventArgs.ExtraDiffStores, loadResources).Load((ModelApplicationBase)Application.Model);
            RuntimeMemberBuilder.AddFields(Application.Model, ((ObjectSpaceProvider)Application.ObjectSpaceProvider).XPDictionary);
        }

        public abstract string GetPath();
    }
}