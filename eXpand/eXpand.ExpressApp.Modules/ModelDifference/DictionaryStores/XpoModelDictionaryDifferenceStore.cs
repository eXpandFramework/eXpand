using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public  class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore
    {
        private readonly bool _enableLoading;
        readonly string _path;
        readonly List<ModelApplicationFromStreamStoreBase> _extraDiffStores;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, bool enableLoading, string path, List<ModelApplicationFromStreamStoreBase> extraDiffStores) : base(application) {
            _enableLoading = enableLoading;
            _path = path;
            _extraDiffStores = extraDiffStores;
        }

        internal bool UseModelFromPath()
        {
            return IsDebuggerAttached && debuggerAttachedEnabled();
        }

        private bool debuggerAttachedEnabled()
        {
            string setting = ConfigurationManager.AppSettings[EnableDebuggerAttachedCheck];
            if (string.IsNullOrEmpty(setting))
                return false;
            return setting.ToLower() == "true";
        }
        
        protected internal List<string> GetModelPaths(){
            List<string> paths = Directory.GetFiles(_path).Where(
                s => s.EndsWith(".xafml")).ToList();
            return paths;
        }

        
        
        public override void Load(ModelApplicationBase model)
        {
            if (!_enableLoading)
                return;
            if (UseModelFromPath()){
                var loadedModel = LoadFromPath();
                loadedModel.Id = "Loaded From Path";
                AddLAyers(new List<ModelApplicationBase> { loadedModel }, model);
                return;
            }
            List<ModelApplicationBase> loadedModels = GetLoadedModelApplications();
            if (loadedModels.Count() == 0){
                loadedModels = new List<ModelApplicationBase> { model.CreatorInstance.CreateModelApplication() };
            }
            AddLAyers(loadedModels, model);
        }

        void AddLAyers(IEnumerable<ModelApplicationBase> loadedModels, ModelApplicationBase model) {
            var language = model.Application.PreferredLanguage;
            foreach (var loadedModel in loadedModels) {
                LoadModelsFromExtraDiffStores(loadedModel,model);
                SaveDifference(loadedModel);
            }
            if (model.Application.PreferredLanguage != language){
                Application.SetLanguage(model.Application.PreferredLanguage);
            }
        }

        void LoadModelsFromExtraDiffStores(ModelApplicationBase loadedModel, ModelApplicationBase model) {
            var extraDiffStores = _extraDiffStores.Where(extraDiffStore => loadedModel.Id == extraDiffStore.Name);
            foreach (var extraDiffStore in extraDiffStores) {  
                model.AddLayer(loadedModel);
                extraDiffStore.Load(loadedModel);
            }
        }

        List<ModelApplicationBase> GetLoadedModelApplications() {
            if (UseModelFromPath()){
                var loadedModel = LoadFromPath();
                loadedModel.Id = "Loaded From Path";
                return new List<ModelApplicationBase> {loadedModel};
            }
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(
                    Application.GetType().FullName, null).ToList().Select(o => o.Model).ToList();
            
        }

        private ModelApplicationBase LoadFromPath()
        {
            var reader = new ModelXmlReader();
            var model = ((ModelApplicationBase)Application.Model).CreatorInstance.CreateModelApplication();

            foreach (var s in GetModelPaths().Where(s => Path.GetFileName(s).ToLower().StartsWith("model") && s.IndexOf(".User") == -1)){
                string replace = s.Replace(".xafml", "");
                string aspect = string.Empty;
                if (replace.IndexOf("_") > -1)
                    aspect = replace.Substring(replace.IndexOf("_") + 1);
                reader.ReadFromFile(model, aspect, s);
            }

            return model;
        }

        public override DifferenceType DifferenceType
        {
            get { return DifferenceType.Model; }
        }

        public bool IsDebuggerAttached{
            get { return Debugger.IsAttached; }
        }


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string modelId) {
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName,modelId);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(ObjectSpace session)
        {
            return new ModelDifferenceObject(ObjectSpace.Session);
        }
    }
}