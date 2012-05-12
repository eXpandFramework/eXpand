using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.ModelDifference.Core;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    internal class ModelDifferenceObjectInfo {
        public ModelDifferenceObjectInfo(ModelDifferenceObject modelDifferenceObject, ModelApplicationBase model) {
            ModelDifferenceObject = modelDifferenceObject;
            Model = model;
        }

        public ModelDifferenceObject ModelDifferenceObject { get; set; }
        public ModelApplicationBase Model { get; set; }
    }
    public class XpoModelDictionaryDifferenceStore : XpoDictionaryDifferenceStore {
        public const string ModelApplicationPrefix = "MDO_";
        public const string RoleApplicationPrefix = "RDO_";
        readonly string _path;
        readonly List<ModelApplicationFromStreamStoreBase> _extraDiffStores;
        readonly bool _loadResources;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, string path, List<ModelApplicationFromStreamStoreBase> extraDiffStores, bool loadResources)
            : base(application) {
            _path = path;
            _extraDiffStores = extraDiffStores;
            _loadResources = loadResources;
        }

        internal bool UseModelFromPath() {
            return IsDebuggerAttached && debuggerAttachedEnabled();
        }

        private bool debuggerAttachedEnabled() {
            string setting = ConfigurationManager.AppSettings[EnableDebuggerAttachedCheck];
            if (string.IsNullOrEmpty(setting))
                return false;
            return setting.ToLower() == "true";
        }

        protected internal List<string> GetModelPaths() {
            List<string> paths = Directory.GetFiles(_path).Where(
                s => s.EndsWith(".xafml")).ToList();
            return paths;
        }



        public override void Load(ModelApplicationBase model) {
            var extraDiffStoresLayerBuilder = new ExtraDiffStoresLayerBuilder();
            var language = model.Application.PreferredLanguage;
            if (UseModelFromPath()) {
                return;
            }
            var loadedModelDifferenceObjectInfos = GetLoadedModelDifferenceObjectInfos(model);
            extraDiffStoresLayerBuilder.AddLayers(loadedModelDifferenceObjectInfos, _extraDiffStores);
            if (_loadResources)
                CreateResourceModels(model, loadedModelDifferenceObjectInfos);
            if (model.Application.PreferredLanguage != language) {
                Application.SetLanguage(model.Application.PreferredLanguage);
            }
            ObjectSpace.CommitChanges();
        }

        Dictionary<string, ModelDifferenceObjectInfo> GetLoadedModelDifferenceObjectInfos(ModelApplicationBase model) {
            Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos = GetLoadedModelApplications(model);

            if (!loadedModelDifferenceObjectInfos.Any()) {
                var modelDifferenceObjectInfos = new Dictionary<string, ModelDifferenceObjectInfo>();
                var application = model.CreatorInstance.CreateModelApplication();
                application.Id = Application.Title;
                model.AddLayerBeforeLast(application);
                var modelDifferenceObject = ObjectSpace.CreateObject<ModelDifferenceObject>().InitializeMembers(application.Id, Application);
                modelDifferenceObjectInfos.Add(application.Id, new ModelDifferenceObjectInfo(modelDifferenceObject, application));
                loadedModelDifferenceObjectInfos = modelDifferenceObjectInfos;
            }
            return loadedModelDifferenceObjectInfos;
        }


        void CreateResourceModels(ModelApplicationBase model, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos) {
            var resourcesLayerBuilder = new ResourcesLayerBuilder(ObjectSpace, Application, this);
            resourcesLayerBuilder.AddLayers(ModelApplicationPrefix, loadedModelDifferenceObjectInfos, model);
            CreateResourceRoleModels(resourcesLayerBuilder, loadedModelDifferenceObjectInfos, model);
        }

        void CreateResourceRoleModels(ResourcesLayerBuilder resourcesLayerBuilder, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, ModelApplicationBase model) {
            var roleMarker = model.CreatorInstance.CreateModelApplication();
            roleMarker.Id = "RoleMarker";
            model.AddLayerBeforeLast(roleMarker);
            resourcesLayerBuilder.AddLayers(RoleApplicationPrefix, loadedModelDifferenceObjectInfos, model);
            var lastLayer = model.LastLayer;
            while (model.LastLayer.Id != "RoleMarker") {
                ModelApplicationHelper.RemoveLayer(model);
            }
            ModelApplicationHelper.RemoveLayer(model);
            ModelApplicationHelper.AddLayer(model, lastLayer);
        }

        Dictionary<string, ModelDifferenceObjectInfo> GetLoadedModelApplications(ModelApplicationBase model) {
            if (UseModelFromPath()) {
                var loadedModel = LoadFromPath();
                loadedModel.Id = "Loaded From Path";
                var modelDifferenceObjectInfos = new Dictionary<string, ModelDifferenceObjectInfo> { { loadedModel.Id, new ModelDifferenceObjectInfo(null, loadedModel) } };
                return modelDifferenceObjectInfos;
            }
            var modelDifferenceObjects = new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName, null);
            return modelDifferenceObjects.ToList().Select(o => new ModelDifferenceObjectInfo(o, o.GetModel(model))).ToDictionary(info => info.Model.Id, objectInfo => objectInfo);

        }

        private ModelApplicationBase LoadFromPath() {
            var reader = new ModelXmlReader();
            var model = ((ModelApplicationBase)Application.Model).CreatorInstance.CreateModelApplication();

            foreach (var s in GetModelPaths().Where(s => (Path.GetFileName(s) + "").ToLower().StartsWith("model") && s.IndexOf(".User", System.StringComparison.Ordinal) == -1)) {
                string replace = s.Replace(".xafml", "");
                string aspect = string.Empty;
                if (replace.IndexOf("_", System.StringComparison.Ordinal) > -1)
                    aspect = replace.Substring(replace.IndexOf("_", System.StringComparison.Ordinal) + 1);
                reader.ReadFromFile(model, aspect, s);
            }

            return model;
        }

        public override DifferenceType DifferenceType {
            get { return DifferenceType.Model; }
        }

        public bool IsDebuggerAttached {
            get { return Debugger.IsAttached; }
        }


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name) {
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName, name);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(IObjectSpace session) {
            return session.CreateObject<ModelDifferenceObject>();
        }
    }
}