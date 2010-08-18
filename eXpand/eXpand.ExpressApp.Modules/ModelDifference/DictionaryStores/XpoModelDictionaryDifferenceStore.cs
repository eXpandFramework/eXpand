using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.Base;
using ResourcesModelStore = eXpand.Persistent.Base.ModelDifference.ResourcesModelStore;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public  class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore
    {
        public const string ModelApplicationPrefix = "MDO_";
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
            ModelApplicationBase lastLayer = model.LastLayer;
            model.RemoveLayer(lastLayer);
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
            CreateResourceModels(model);
            model.AddLayer(lastLayer);
        }

        void CreateResourceModels(ModelApplicationBase model) {
            
            var assemblies = ((IModelApplicationModule) model.Application).ModulesList.Select(module => XafTypesInfo.Instance.FindTypeInfo(module.Name).AssemblyInfo.Assembly);
            foreach (var assembly in assemblies) {
                LoadFromResources(model, ModelApplicationPrefix, assembly);
            }
            
        }

        void LoadFromResources(ModelApplicationBase model, string modelApplicationPrefix, Assembly assembly) {
            var resourcesModelStore = new ResourcesModelStore(assembly, modelApplicationPrefix);
            ModelDifferenceObject modelDifferenceObject = null;
            resourcesModelStore.ResourceLoading += (sender, args) =>{
                var resourceName = Path.GetFileNameWithoutExtension(args.ResourceName.StartsWith(modelApplicationPrefix) ? args.ResourceName : args.ResourceName.Substring(args.ResourceName.IndexOf("." + modelApplicationPrefix) + 1)).Replace(modelApplicationPrefix,"");
                if (GetActiveDifferenceObject(resourceName) != null) {
                    modelDifferenceObject = GetActiveDifferenceObject(resourceName);
                    model.AddLayer(modelDifferenceObject.Model);
                }
                else {
                    modelDifferenceObject =new ModelDifferenceObject(ObjectSpace.Session).InitializeMembers(resourceName, Application.Title,Application.GetType().FullName);
                }
                args.Model = modelDifferenceObject.Model;
            };
            resourcesModelStore.ResourceLoaded += (o, loadedArgs) =>{
                if (modelDifferenceObject.Model.HasModification){
                    ObjectSpace.SetModified(modelDifferenceObject);
                    ObjectSpace.CommitChanges();
                }
            };
            resourcesModelStore.Load(model);
        }

        void AddLAyers(IEnumerable<ModelApplicationBase> loadedModels, ModelApplicationBase model) {
            var language = model.Application.PreferredLanguage;
            Tracing.Tracer.LogVerboseSubSeparator("ModelDifference -- Adding Layers to application model ");
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
                Tracing.Tracer.LogVerboseValue("Name",extraDiffStore.Name);
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


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name) {
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName,name);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(ObjectSpace session)
        {
            return new ModelDifferenceObject(ObjectSpace.Session);
        }
    }
}