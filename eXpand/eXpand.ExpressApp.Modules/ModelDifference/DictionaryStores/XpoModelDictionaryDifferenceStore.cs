using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using eXpand.ExpressApp.Core;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;
using eXpand.ExpressApp.SystemModule;
using eXpand.Persistent.Base;
using ResourcesModelStore = eXpand.Persistent.Base.ModelDifference.ResourcesModelStore;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores{
    public  class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore
    {
        public const string ModelApplicationPrefix = "MDO_";
        public const string RoleApplicationPrefix = "RDO_";
        readonly string _path;
        readonly List<ModelApplicationFromStreamStoreBase> _extraDiffStores;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, string path, List<ModelApplicationFromStreamStoreBase> extraDiffStores) : base(application) {
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
        }

        void CreateResourceModels(ModelApplicationBase model) {
            
            var assemblies = ((IModelApplicationModule) model.Application).ModulesList.Select(module => XafTypesInfo.Instance.FindTypeInfo(module.Name).AssemblyInfo.Assembly);
            foreach (var assembly in assemblies) {
                LoadFromResources(model, ModelApplicationPrefix, assembly);
                LoadFromResources(model, RoleApplicationPrefix, assembly);
            }
            
        }

        void LoadFromResources(ModelApplicationBase model, string prefix, Assembly assembly) {
            var resourcesModelStore = new ResourcesModelStore(assembly, prefix);
            ModelDifferenceObject modelDifferenceObject = null;
            resourcesModelStore.ResourceLoading += (sender, args) =>{
                var resourceName = Path.GetFileNameWithoutExtension(args.ResourceName.StartsWith(prefix) ? args.ResourceName : args.ResourceName.Substring(args.ResourceName.IndexOf("." + prefix) + 1)).Replace(prefix,"");
                var activeDifferenceObject = GetDifferenceObject(resourceName,prefix);
                if (activeDifferenceObject != null) {
                    modelDifferenceObject = activeDifferenceObject;
                    model.AddLayerBeforeLast(modelDifferenceObject.Model);
                }
                else {
                    modelDifferenceObject = GetModelDifferenceObject(resourceName,prefix);
                }
                args.Model = modelDifferenceObject.Model;
            };
            resourcesModelStore.ResourceLoaded += (o, loadedArgs) =>{
                if (!(modelDifferenceObject.Model.IsEmpty)){
                    ObjectSpace.SetModified(modelDifferenceObject);
                }
            };
            resourcesModelStore.Load(model);
            ObjectSpace.CommitChanges();
        }

        ModelDifferenceObject GetDifferenceObject(string resourceName, string prefix) {
            if (prefix==ModelApplicationPrefix)
                return GetActiveDifferenceObject(resourceName);
            return ObjectSpace.Session.FindObject<RoleModelDifferenceObject>(o => o.Name == resourceName);
        }

        ModelDifferenceObject GetModelDifferenceObject(string resourceName, string prefix) {
            ModelDifferenceObject modelDifferenceObject;
            if (prefix == ModelApplicationPrefix)
                modelDifferenceObject = new ModelDifferenceObject(ObjectSpace.Session);
            else {
                modelDifferenceObject = new RoleModelDifferenceObject(ObjectSpace.Session);
                Type roleType = ((ISecurityComplex) SecuritySystem.Instance).RoleType;
                var criteriaParametersList = resourceName.Substring(0,resourceName.IndexOf("_"));
                object findObject = ObjectSpace.FindObject(roleType,CriteriaOperator.Parse("Name=?",criteriaParametersList));
                Guard.ArgumentNotNull(findObject, criteriaParametersList);
                var xpBaseCollection = ((XPBaseCollection) modelDifferenceObject.GetMemberValue("Roles"));
                xpBaseCollection.BaseAdd(findObject);
            }
            modelDifferenceObject.InitializeMembers(resourceName, Application.Title, Application.GetType().FullName);
            return modelDifferenceObject;
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
                model.AddLayerBeforeLast(loadedModel);
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