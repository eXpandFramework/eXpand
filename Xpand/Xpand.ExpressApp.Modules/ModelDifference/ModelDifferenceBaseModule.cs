using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.ModelDifference.DictionaryStores;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.RuntimeMembers;

namespace Xpand.ExpressApp.ModelDifference {
    public abstract class ModelDifferenceBaseModule : XpandModuleBase {
        XpoUserModelDictionaryDifferenceStore _userModelDictionaryDifferenceStore;

        protected abstract bool? ModelsLoaded { get; set; }
        public event EventHandler<CreateCustomModelDifferenceStoreEventArgs> CreateCustomModelDifferenceStore;

        public void OnCreateCustomModelDifferenceStore(CreateCustomModelDifferenceStoreEventArgs e) {
            EventHandler<CreateCustomModelDifferenceStoreEventArgs> handler = CreateCustomModelDifferenceStore;
            if (handler != null) handler(this, e);
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
            LoadModels(!ModelsLoaded.HasValue || !ModelsLoaded.Value);
        }

        public void LoadModels(bool loadResources) {
            var model = (ModelApplicationBase)Application.Model;
            LoadApplicationModels(loadResources, model);
            if (Application.Security is ISecurityComplex && _userModelDictionaryDifferenceStore != null)
                _userModelDictionaryDifferenceStore.Load();
        }

        void LoadApplicationModels(bool loadResources, ModelApplicationBase model) {
            var userDiffLayer = model.LastLayer;
            ModelApplicationHelper.RemoveLayer(model);
            var customModelDifferenceStoreEventArgs = new CreateCustomModelDifferenceStoreEventArgs();
            OnCreateCustomModelDifferenceStore(customModelDifferenceStoreEventArgs);
            if (!customModelDifferenceStoreEventArgs.Handled)
                new XpoModelDictionaryDifferenceStore(Application, GetPath(), customModelDifferenceStoreEventArgs.ExtraDiffStores, loadResources).Load(model);
            ModelApplicationHelper.AddLayer((ModelApplicationBase)Application.Model, userDiffLayer);
            RuntimeMemberBuilder.CreateRuntimeMembers(Application.Model);
        }

        public abstract string GetPath();
    }
}