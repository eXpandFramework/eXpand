using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference {
    public abstract class ModelDifferenceBaseModule : XpandModuleBase {
        public abstract bool? ModelsLoaded { get; set; }
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
                Application.LoggedOff += Application_LoggedOff;
                Application.Disposed += Application_Disposed;
            }
        }

        void Application_Disposed(object sender, EventArgs e) {
            ((XafApplication)sender).Disposed -= Application_Disposed;
            ((XafApplication)sender).LoggedOff -= Application_LoggedOff;
        }

        void Application_LoggedOff(object sender, EventArgs e) {
            var modelApplicationBase = ((ModelApplicationBase)((XafApplication)sender).Model);
            var lastLayer = modelApplicationBase.LastLayer;
            while (lastLayer.Id != "Unchanged Master Part") {
                modelApplicationBase.RemoveLayer(lastLayer);
                lastLayer = modelApplicationBase.LastLayer;
            }
            var afterSetupLayer = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetupLayer.Id = "After Setup";
            modelApplicationBase.AddLayer(afterSetupLayer);
        }

        public void LoadModels(bool loadResources) {
            ((ModelApplicationBase)Application.Model).ReInitLayers();
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled)
                new XpoModelDictionaryDifferenceStore(Application, GetPath(), customModelDifferenceStoreEventArgs.ExtraDiffStores, loadResources).Load((ModelApplicationBase)Application.Model);
        }

        public abstract string GetPath();
    }
}