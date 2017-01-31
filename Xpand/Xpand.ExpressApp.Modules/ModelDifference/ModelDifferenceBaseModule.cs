using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers;

namespace Xpand.ExpressApp.ModelDifference {
    public abstract class ModelDifferenceBaseModule : XpandModuleBase {
        XpoUserModelDictionaryDifferenceStore _userModelDictionaryDifferenceStore;

        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        public void OnCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e) {
            EventHandler<CreateCustomModelDifferenceStoreEventArgs> handler = CreateCustomModelDifferenceStore;
            handler?.Invoke(this, e);
        }

        public override void Setup(ApplicationModulesManager moduleManager) {
            base.Setup(moduleManager);
            if (Application != null) {
                Application.UserDifferencesLoaded += OnUserDifferencesLoaded;
                Application.CreateCustomUserModelDifferenceStore += ApplicationOnCreateCustomUserModelDifferenceStore;
            }
        }

        void ApplicationOnCreateCustomUserModelDifferenceStore(object sender, DevExpress.ExpressApp.CreateCustomModelDifferenceStoreEventArgs createCustomModelDifferenceStoreEventArgs) {
            createCustomModelDifferenceStoreEventArgs.Handled = true;
            _userModelDictionaryDifferenceStore =_userModelDictionaryDifferenceStore?? new XpoUserModelDictionaryDifferenceStore(Application);
            createCustomModelDifferenceStoreEventArgs.Store = _userModelDictionaryDifferenceStore;
        }

        void OnUserDifferencesLoaded(object sender, EventArgs eventArgs) {
            LoadModels();
        }

        public void LoadModels() {
            var model = (ModelApplicationBase)Application.Model;
            LoadApplicationModels(model);
            if (Application.Security is ISecurityComplex)
                _userModelDictionaryDifferenceStore?.Load();
            RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
        }

        void LoadApplicationModels(ModelApplicationBase model) {
            var userDiffLayers = new List<ModelApplicationBase>();
            while (model.LastLayer != null && model.LastLayer.Id != "After Setup"){
                userDiffLayers.Add(model.LastLayer);
                ModelApplicationHelper.RemoveLayer(model);
            }
            if (model.LastLayer == null)
                throw new ArgumentException("Model.LastLayer null");
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled){
                new XpoModelDictionaryDifferenceStore(Application, GetPath(), customModelDifferenceStoreEventArgs.ExtraDiffStores).Load(model);
            }
            userDiffLayers.Reverse();
            foreach (var layer in userDiffLayers){
                ModelApplicationHelper.AddLayer((ModelApplicationBase)Application.Model, layer);
            }
        }

        public abstract string GetPath();
    }
}
