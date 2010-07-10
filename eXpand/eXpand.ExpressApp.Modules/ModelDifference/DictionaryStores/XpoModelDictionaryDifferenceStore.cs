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
        readonly List<KeyValuePair<string, ModelStoreBase>> _extraDiffStores;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, bool enableLoading, string path, List<KeyValuePair<string, ModelStoreBase>> extraDiffStores) : base(application) {
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
          
            ModelApplicationBase loadedModel = GetLoadedModel();

            if (loadedModel != null){
                var language = model.Application.PreferredLanguage;
                var userLayer = model.LastLayer;
                model.RemoveLayer(userLayer);
                if (LoadModelsFromExtraDiffStores(loadedModel))
                    SaveDifference(loadedModel);
                model.AddLayer(loadedModel);
                model.AddLayer(userLayer);
                if (model.Application.PreferredLanguage != language){
                    Application.SetLanguage(model.Application.PreferredLanguage);
                }
            }
            else {
                ModelApplicationBase modelApplicationBase = model.LastLayer;
                LoadModelsFromExtraDiffStores(modelApplicationBase);
                model.AddLayer(modelApplicationBase);
                SaveDifference(model.LastLayer);
            }
        }

        bool LoadModelsFromExtraDiffStores(ModelApplicationBase modelApplicationBase) {
            foreach (var extraDiffStore in _extraDiffStores) {
                extraDiffStore.Value.Load(modelApplicationBase);
                
            }
            return modelApplicationBase.HasModification;
        }

        ModelApplicationBase GetLoadedModel() {
            var activeDifferenceObject = GetActiveDifferenceObject();
            ModelApplicationBase loadedModel = null;

            if (UseModelFromPath()){
                loadedModel = LoadFromPath();
                loadedModel.Id = "Loaded From Path";
            }
            else if (activeDifferenceObject != null){
                loadedModel = activeDifferenceObject.Model;
                loadedModel.Id = activeDifferenceObject.Name;
            }
            return loadedModel;
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

        protected internal override ModelDifferenceObject GetActiveDifferenceObject(){
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(ObjectSpace session)
        {
            return new ModelDifferenceObject(ObjectSpace.Session);
        }
    }
}