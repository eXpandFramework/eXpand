using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.ModelDifference;

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
        readonly List<ModelApplicationFromStreamStoreBase> _extraDiffStores;
        private static bool _executed;
        public const string EnableDebuggerAttachedCheck = "EnableDebuggerAttachedCheck";

        public XpoModelDictionaryDifferenceStore(XafApplication application, List<ModelApplicationFromStreamStoreBase> extraDiffStores)
            : base(application) {
            _extraDiffStores = extraDiffStores;
        }

        public override void Load(ModelApplicationBase model) {
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Adding Layers to application model ");
            var extraDiffStoresLayerBuilder = new ExtraDiffStoresLayerBuilder();
            var language = model.Application.PreferredLanguage;
            
            var loadedModelDifferenceObjectInfos = GetLoadedModelDifferenceObjectInfos(model);
            
            extraDiffStoresLayerBuilder.AddLayers(loadedModelDifferenceObjectInfos, _extraDiffStores);
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Loaded: "+_executed);
            if (!_executed) {
                KeyValuePair<string, ModelDifferenceObjectInfo> valuePair = loadedModelDifferenceObjectInfos.FirstOrDefault(pair 
                    => IsUpdateableFromFile(model, pair));
                if (!Equals(valuePair, default(KeyValuePair<string, ModelDifferenceObjectInfo>))) {
                    var applicationFolder = PathHelper.GetApplicationFolder();
                    valuePair.Value.ModelDifferenceObject.CreateAspectsFromPath(Application.GetDiffDefaultName(applicationFolder));
                    Tracing.Tracer.LogVerboseValue("ObjectSpace.ModifiedObjects", ObjectSpace.ModifiedObjects.Count);
                }
                Tracing.Tracer.LogSubSeparator("ModelDifference -- CreateResourceModels");
                CreateResourceModels(model, loadedModelDifferenceObjectInfos);
                _executed = true;
            }
            if (model.Application.PreferredLanguage != language) {
                Application.SetLanguage(model.Application.PreferredLanguage);
            }
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Layers added to application model");
            ObjectSpace.CommitChanges();
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Application model saved to the database");
            
        }

        private bool IsUpdateableFromFile(ModelApplicationBase model, KeyValuePair<string, ModelDifferenceObjectInfo> pair){
            var objectType = pair.Value.ModelDifferenceObject.GetType();
            var criteria = CriteriaOperator.Parse(((IModelOptionsModelDifference) model.Application.Options).ModelToUpdateFromFileCriteria);
            if (!ReferenceEquals(criteria,null)){
                var isObjectFitForCriteria = ObjectSpace.IsObjectFitForCriteria(objectType, pair.Value.ModelDifferenceObject,criteria);
                var isFit = isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value;
                Tracing.Tracer.LogSubSeparator("ModelDifference -- Criteria: " + criteria.ToString() + ", -- object: " + pair.Value.ModelDifferenceObject+", --fit:"+isFit);
                return isFit;
            }
            return false;
        }

        Dictionary<string, ModelDifferenceObjectInfo> GetLoadedModelDifferenceObjectInfos(ModelApplicationBase model) {
            Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos = GetLoadedModelApplications(model);
            if (!loadedModelDifferenceObjectInfos.Any())
                if (ObjectSpace.IsServerSide() || !(Application is ServerApplication))
                    return CreateNew(model);
                else
                    return loadedModelDifferenceObjectInfos;
            return loadedModelDifferenceObjectInfos;
        }

        Dictionary<string, ModelDifferenceObjectInfo> CreateNew(ModelApplicationBase model) {
            var modelDifferenceObjectInfos = new Dictionary<string, ModelDifferenceObjectInfo>();
            var application = CreateModelApplication(model, DifferenceType);
            model.AddLayerBeforeLast(application);
            var modelDifferenceObject = ObjectSpace.CreateObject<ModelDifferenceObject>().InitializeMembers(application.Id, Application);
            modelDifferenceObject.CreateAspectsFromPath(Application.GetDiffDefaultName(PathHelper.GetApplicationFolder()));
            CreateUserModelDifferenceXPObjectType();
            var modelDifferenceObjectInfo = new ModelDifferenceObjectInfo(modelDifferenceObject, application);
            modelDifferenceObjectInfos.Add(application.Id, modelDifferenceObjectInfo);
            return modelDifferenceObjectInfos;
        }

        void CreateUserModelDifferenceXPObjectType() {
            if (Application is ServerApplication &&ObjectSpace.FindObject<XPObjectType>(CriteriaOperator.Parse("TypeName=?",typeof (UserModelDifferenceObject).FullName)) ==null) {
                var xpObjectType = ObjectSpace.CreateObject<XPObjectType>();
                xpObjectType.TypeName = typeof (UserModelDifferenceObject).FullName;
                xpObjectType.AssemblyName = typeof (UserModelDifferenceObject).Assembly.GetName().Name;
            }
        }

        protected ModelApplicationBase CreateModelApplication(ModelApplicationBase model, DifferenceType differenceType) {
            var application = model.CreatorInstance.CreateModelApplication();
            application.Id = Application.Title;
            return application;
        }


        void CreateResourceModels(ModelApplicationBase model, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos) {
            var resourcesLayerBuilder = new ResourcesLayerBuilder(ObjectSpace, Application, this);
            resourcesLayerBuilder.AddLayers(ModelApplicationPrefix, loadedModelDifferenceObjectInfos, model);
            CreateResourceRoleModels(resourcesLayerBuilder, loadedModelDifferenceObjectInfos, model);
        }

        void CreateResourceRoleModels(ResourcesLayerBuilder resourcesLayerBuilder, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, ModelApplicationBase model) {
            var roleMarker = CreateModelApplication(model,DifferenceType.Role);
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
            var modelDifferenceObjects = new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName, null);
            return modelDifferenceObjects.ToList().Select(o => new ModelDifferenceObjectInfo(o, o.GetModel(model))).ToDictionary(info => info.Model.Id, objectInfo => objectInfo);

        }

        public override DifferenceType DifferenceType => DifferenceType.Model;

        public bool IsDebuggerAttached => Debugger.IsAttached;


        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name) {
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName, name);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(IObjectSpace session) {
            return session.CreateObject<ModelDifferenceObject>();
        }
    }
}