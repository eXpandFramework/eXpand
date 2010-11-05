using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.SystemModule;
using Xpand.Persistent.Base.ModelDifference;
using Xpand.Xpo;

namespace Xpand.ExpressApp.ModelDifference.DictionaryStores {
    internal class ResourcesLayerBuilder {
        readonly ObjectSpace _objectSpace;
        readonly XafApplication _xafApplication;
        

        public ResourcesLayerBuilder(ObjectSpace objectSpace, XafApplication xafApplication) {
            _objectSpace = objectSpace;
            _xafApplication = xafApplication;
        }



        public void AddLayers(string modelApplicationPrefix, IModelModules modelModules, ModelApplicationBase model) {
            AddLayers(model, modelModules, modelApplicationPrefix, new Dictionary<string, ModelDifferenceObjectInfo>(),null);
        }


        public void AddLayers(string modelApplicationPrefix, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, ModelApplicationBase model,XpoModelDictionaryDifferenceStore xpoModelDictionaryDifferenceStore) {
            IModelModules modelModules = ((IModelApplicationModule)model.Application).ModulesList;
            AddLayers(model, modelModules, modelApplicationPrefix, loadedModelDifferenceObjectInfos, xpoModelDictionaryDifferenceStore);
        }

        void AddLayers(ModelApplicationBase model, IEnumerable<IModelModule> modelModules, string modelApplicationPrefix, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, XpoModelDictionaryDifferenceStore xpoModelDictionaryDifferenceStore) {
            var modelXmlReader = new ModelXmlReader();
            var assemblies = modelModules.Select(module => XafTypesInfo.Instance.FindTypeInfo(module.Name).AssemblyInfo.Assembly);
            var resourceModelCollector = new ResourceModelCollector();
            foreach (var keyValuePair in resourceModelCollector.Collect(assemblies, modelApplicationPrefix)) {
                var modelDifferenceObjectInfo = GetModelDifferenceObjectInfo(modelApplicationPrefix, loadedModelDifferenceObjectInfos, keyValuePair.Key, model, xpoModelDictionaryDifferenceStore);
                if (modelDifferenceObjectInfo!=null) {
                    foreach (var aspectInfo in keyValuePair.Value.AspectInfos) {
                        modelXmlReader.ReadFromString(modelDifferenceObjectInfo.Model, aspectInfo.AspectName, aspectInfo.Xml);
                    }
                    modelDifferenceObjectInfo.ModelDifferenceObject.CreateAspects(modelDifferenceObjectInfo.Model);
                }
            }
        }

        ModelDifferenceObject CreateDifferenceObject(string resourceName, string prefix) {
            ModelDifferenceObject modelDifferenceObject;
            if (prefix == XpoModelDictionaryDifferenceStore.ModelApplicationPrefix)
                modelDifferenceObject = new ModelDifferenceObject(_objectSpace.Session);
            else {
                modelDifferenceObject = new RoleModelDifferenceObject(_objectSpace.Session);
                Type roleType = ((ISecurityComplex)SecuritySystem.Instance).RoleType;
                var criteriaParametersList = resourceName.Substring(0, resourceName.IndexOf("_"));
                object findObject = _objectSpace.FindObject(roleType, CriteriaOperator.Parse("Name=?", criteriaParametersList));
                Guard.ArgumentNotNull(findObject, criteriaParametersList);
                var xpBaseCollection = ((XPBaseCollection)modelDifferenceObject.GetMemberValue("Roles"));
                xpBaseCollection.BaseAdd(findObject);
            }
            modelDifferenceObject.InitializeMembers(resourceName, _xafApplication.Title, _xafApplication.GetType().FullName);
            return modelDifferenceObject;
        }

        ModelDifferenceObject FindDifferenceObject(string resourceName, string prefix, XpoModelDictionaryDifferenceStore xpoModelDictionaryDifferenceStore) {
            if (prefix == XpoModelDictionaryDifferenceStore.ModelApplicationPrefix)
                return xpoModelDictionaryDifferenceStore.GetActiveDifferenceObject(resourceName);
            return _objectSpace.Session.FindObject<RoleModelDifferenceObject>(o => o.Name == resourceName);
        }

        ModelDifferenceObjectInfo GetModelDifferenceObjectInfo(string prefix, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, string resourceName, ModelApplicationBase model, XpoModelDictionaryDifferenceStore xpoModelDictionaryDifferenceStore) {
            ModelDifferenceObject activeDifferenceObject;
            ModelApplicationBase modelApplicationBase;
            if (!loadedModelDifferenceObjectInfos.ContainsKey(resourceName)) {
                activeDifferenceObject = FindDifferenceObject(resourceName, prefix, xpoModelDictionaryDifferenceStore) ??
                                         CreateDifferenceObject(resourceName, prefix);
                modelApplicationBase = activeDifferenceObject.GetModel(model);
            } else {
                var loadedModelDifferenceObjectInfo = loadedModelDifferenceObjectInfos[resourceName];
                activeDifferenceObject = loadedModelDifferenceObjectInfo.ModelDifferenceObject;
                modelApplicationBase = loadedModelDifferenceObjectInfo.Model;
            }
            return new ModelDifferenceObjectInfo(activeDifferenceObject, modelApplicationBase);
        }

    }
}