using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.Core;
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
            
            var objectInfos = GetObjectInfos(model, DeviceCategory.All);
            if (DeviceModelsEnabled)
                objectInfos= objectInfos.Concat(GetObjectInfos(model, Application.GetDeviceCategory())).ToDictionary(pair => pair.Key, pair => pair.Value);


            extraDiffStoresLayerBuilder.AddLayers(objectInfos, _extraDiffStores);
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Loaded: "+_executed);
            if (!_executed) {
                UpdateModelFromPath(model, objectInfos);
                Tracing.Tracer.LogSubSeparator("ModelDifference -- CreateResourceModels");
                CreateResourceModels(model, objectInfos);
                _executed = true;
            }
            if (model.Application.PreferredLanguage != language) {
                Application.SetLanguage(model.Application.PreferredLanguage);
            }
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Layers added to application model");
            ObjectSpace.CommitChanges();
            Tracing.Tracer.LogSubSeparator("ModelDifference -- Application model saved to the database");
            
        }

        private void UpdateModelFromPath(ModelApplicationBase model, Dictionary<string, ModelDifferenceObjectInfo> objectInfos) {
            var modelOptionsModelDifference = ((IModelOptionsModelDifference)model.Application.Options);
            var applicationFolder = PathHelper.GetApplicationFolder();
            var keyValuePairs = new[] {
                new KeyValuePair<KeyValuePair<DeviceCategory?, string>, string>(
                    new KeyValuePair<DeviceCategory?, string>(null, Application.GetDiffDefaultName(applicationFolder)),
                    modelOptionsModelDifference.ModelToUpdateFromFileCriteria),
                new KeyValuePair<KeyValuePair<DeviceCategory?, string>, string>(
                    new KeyValuePair<DeviceCategory?, string>(DeviceCategory.Tablet, AppDiffDefaultTabletName),
                    modelOptionsModelDifference.ModelToUpdateFromTabletFileCriteria),
                new KeyValuePair<KeyValuePair<DeviceCategory?, string>, string>(
                    new KeyValuePair<DeviceCategory?, string>(DeviceCategory.Desktop, AppDiffDefaultDesktopName),
                    modelOptionsModelDifference.ModelToUpdateFromDesktopFileCriteria),
                new KeyValuePair<KeyValuePair<DeviceCategory?, string>, string>(
                    new KeyValuePair<DeviceCategory?, string>(DeviceCategory.Mobile, AppDiffDefaultMobileName),
                    modelOptionsModelDifference.ModelToUpdateFromMobileFileCriteria)
            };
            foreach (var keyValuePair in keyValuePairs) {
                var valuePair = objectInfos.FirstOrDefault(pair => IsUpdateableFromFile(pair, keyValuePair));
                if (!Equals(valuePair, default(KeyValuePair<string, ModelDifferenceObjectInfo>))) {
	                if (((IModelOptionsModelDifference)model.Application.Options).ModelUpdateMode == ModelUpdateMode.Never)
		                throw new NotSupportedException($"You cannot modify a model while {nameof(IModelOptionsModelDifference.ModelUpdateMode)}={ModelUpdateMode.Never}");
					valuePair.Value.ModelDifferenceObject.CreateAspectsFromPath(keyValuePair.Key.Value);
                    Tracing.Tracer.LogVerboseValue("ObjectSpace.ModifiedObjects", ObjectSpace.ModifiedObjects.Count);
                }
            }
        }

        private bool IsUpdateableFromFile(KeyValuePair<string, ModelDifferenceObjectInfo> pair, KeyValuePair<KeyValuePair<DeviceCategory?, string>, string> keyValuePair) {
            var objectType = pair.Value.ModelDifferenceObject.GetType();
            var criteria = CriteriaOperator.Parse(keyValuePair.Value);
            if (!ReferenceEquals(criteria,null)){
                var isObjectFitForCriteria = ObjectSpace.IsObjectFitForCriteria(objectType, pair.Value.ModelDifferenceObject,criteria);
                var isFit = isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value;
                return isFit && pair.Value.ModelDifferenceObject.DeviceCategory == keyValuePair.Key.Key;
            }
            return false;
        }

        Dictionary<string, ModelDifferenceObjectInfo> GetObjectInfos(ModelApplicationBase model,DeviceCategory deviceCategory) {
            Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos = GetModels(model,deviceCategory);
            if (!loadedModelDifferenceObjectInfos.Any())
                if (ObjectSpace.IsServerSide() || !(Application is ServerApplication))
                    return CreateNew(model,deviceCategory);
                else
                    return loadedModelDifferenceObjectInfos;
            return loadedModelDifferenceObjectInfos;
        }

        Dictionary<string, ModelDifferenceObjectInfo> CreateNew(ModelApplicationBase model, DeviceCategory deviceCategory) {
			if (((IModelOptionsModelDifference) model.Application.Options).ModelUpdateMode==ModelUpdateMode.Never)
				throw new NotSupportedException($"You cannot create a new application model while {nameof(IModelOptionsModelDifference.ModelUpdateMode)}={ModelUpdateMode.Never}");
            var modelDifferenceObjectInfos = new Dictionary<string, ModelDifferenceObjectInfo>();
            var application = CreateModelApplication(model, DifferenceType,deviceCategory);
            model.AddLayerBeforeLast(application);
            var modelDifferenceObject = ObjectSpace.CreateObject<ModelDifferenceObject>().InitializeMembers(Application.Title, Application,deviceCategory);
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

        protected ModelApplicationBase CreateModelApplication(ModelApplicationBase model, DifferenceType differenceType, DeviceCategory deviceCategory) {
            var application = model.CreatorInstance.CreateModelApplication();
            application.Id = $"{Application.Title}-{deviceCategory}";
            return application;
        }


        void CreateResourceModels(ModelApplicationBase model, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos) {
            var resourcesLayerBuilder = new ResourcesLayerBuilder(ObjectSpace, Application, this);
            resourcesLayerBuilder.AddLayers(ModelApplicationPrefix, loadedModelDifferenceObjectInfos, model);
            CreateResourceRoleModels(resourcesLayerBuilder, loadedModelDifferenceObjectInfos, model);
        }

        void CreateResourceRoleModels(ResourcesLayerBuilder resourcesLayerBuilder, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, ModelApplicationBase model) {
            var roleMarker = CreateModelApplication(model,DifferenceType.Role,DeviceCategory.All);
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

        Dictionary<string, ModelDifferenceObjectInfo> GetModels(ModelApplicationBase model, DeviceCategory deviceCategory) {
            var modelDifferenceObjects = new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifferences(Application.GetType().FullName, null,deviceCategory);
            return modelDifferenceObjects.ToList().Select(o => new ModelDifferenceObjectInfo(o, o.GetModel(model))).ToDictionary(info => info.Model.Id, objectInfo => objectInfo);

        }

        public override DifferenceType DifferenceType => DifferenceType.Model;

        public bool IsDebuggerAttached => Debugger.IsAttached;


        protected override bool DeviceModelsEnabled => Application.ApplicationDeviceModelsEnabled();

        protected internal override ModelDifferenceObject GetActiveDifferenceObject(string name,DeviceCategory deviceCategory) {
            return new QueryModelDifferenceObject(ObjectSpace.Session).GetActiveModelDifference(Application.GetType().FullName, name,deviceCategory);
        }

        protected internal override ModelDifferenceObject GetNewDifferenceObject(IObjectSpace session) {
            return session.CreateObject<ModelDifferenceObject>();
        }
    }
}