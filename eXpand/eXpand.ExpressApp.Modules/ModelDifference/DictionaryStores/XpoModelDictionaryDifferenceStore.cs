using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public  class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore
    {
        private readonly bool _enableLoading;
        readonly string _path;
        readonly List<ModelFromResourceStoreBase> _extraDiffStores;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, bool enableLoading, string path, List<ModelFromResourceStoreBase> extraDiffStores) : base(application) {
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
            List<ModelApplicationBase> loadedModels = GetLoadedModels();
            if (loadedModels.Count() == 0){
                ModelApplicationBase modelApplicationBase = model.LastLayer;
                loadedModels = new List<ModelApplicationBase> {modelApplicationBase};
            }
            AddLAyers(loadedModels, model);
        }

        void AddLAyers(IEnumerable<ModelApplicationBase> loadedModels, ModelApplicationBase model) {
            var language = model.Application.PreferredLanguage;
            var userLayer = model.LastLayer;
            model.RemoveLayer(userLayer);
            foreach (var loadedModel in loadedModels) {
                LoadModelsFromExtraDiffStores(loadedModel);
                SaveDifference(loadedModel);
                model.AddLayer(loadedModel);    
            }
            model.AddLayer(userLayer);
            if (model.Application.PreferredLanguage != language){
                Application.SetLanguage(model.Application.PreferredLanguage);
            }
        }

        void LoadModelsFromExtraDiffStores(ModelApplicationBase modelApplicationBase) {
            var extraDiffStores = _extraDiffStores.Where(extraDiffStore => modelApplicationBase.Id == extraDiffStore.Name);
            foreach (var extraDiffStore in extraDiffStores) {
                extraDiffStore.Load(modelApplicationBase);
            }
        }

        List<ModelApplicationBase> GetLoadedModels() {
            var modelApplicationBases = new List<ModelApplicationBase>();
            IQueryable<ModelDifferenceObject> activeDifferenceObject = new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName,null);
            ModelApplicationBase loadedModel;

            if (UseModelFromPath()){
                loadedModel = LoadFromPath();
                loadedModel.Id = "Loaded From Path";
            }
            else {
                foreach (var modelDifferenceObject in activeDifferenceObject) {
                    ModelApplicationBase modelApplicationBase = modelDifferenceObject.Model;
                    modelApplicationBase.Id = modelDifferenceObject.ModelId;
                    modelApplicationBases.Add(modelApplicationBase);
                }
            }
            return modelApplicationBases;
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