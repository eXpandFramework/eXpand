using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.SystemModule;
using eXpand.Xpo;

namespace eXpand.ExpressApp.ModelDifference.DictionaryStores {
    internal class ResourcesLayerBuilder {
        readonly ObjectSpace _objectSpace;
        readonly XafApplication _xafApplication;
        readonly XpoModelDictionaryDifferenceStore _xpoModelDictionaryDifferenceStore;

        public ResourcesLayerBuilder(ObjectSpace objectSpace,XafApplication xafApplication,XpoModelDictionaryDifferenceStore xpoModelDictionaryDifferenceStore) {
            _objectSpace = objectSpace;
            _xafApplication = xafApplication;
            _xpoModelDictionaryDifferenceStore = xpoModelDictionaryDifferenceStore;
        }

        Dictionary<string,List<string>> Collect(IEnumerable<IModelModule> modelModules, string prefix){
            var assemblies = modelModules.Select(module => XafTypesInfo.Instance.FindTypeInfo(module.Name).AssemblyInfo.Assembly);
            var assemblyResourcesNames = assemblies.SelectMany(assembly => assembly.GetManifestResourceNames(), (assembly1, s) => new { assembly1, s });
            assemblyResourcesNames =assemblyResourcesNames.Where(arg =>((arg.s.StartsWith(prefix) || (!(arg.s.StartsWith(prefix)) && arg.s.IndexOf("." + prefix) > -1))));
            var dictionary = new Dictionary<string, List<string>>();
            foreach (var assemblyResourcesName in assemblyResourcesNames) {
                var resourceName = assemblyResourcesName.s;
                resourceName=Path.GetFileNameWithoutExtension(resourceName.StartsWith(prefix) ? resourceName
                                                     : resourceName.Substring(resourceName.IndexOf("." + prefix) + 1)).Replace(prefix, "");
                if (!(dictionary.ContainsKey(resourceName)))
                    dictionary.Add(resourceName, new List<string>());
                var assembly1 = assemblyResourcesName.assembly1;
                var xml = GetXml(assemblyResourcesName.s, assembly1);
                dictionary[resourceName].Add(xml);
            }
            return dictionary;
        }

        string GetXml(string resourceName, Assembly assembly1) {
            string readToEnd;
            using (var manifestResourceStream = assembly1.GetManifestResourceStream(resourceName))
            {
                if (manifestResourceStream == null) throw new NullReferenceException(resourceName);
                using (var streamReader = new StreamReader(manifestResourceStream)) {
                    readToEnd = streamReader.ReadToEnd();
                }
            }
            return readToEnd;
        }

        public void AddLayers(string modelApplicationPrefix, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, ModelApplicationBase model) {
            var modelXmlReader = new ModelXmlReader();
            var modulesList = ((IModelApplicationModule) model.Application).ModulesList;
            foreach (var resourse in Collect(modulesList,modelApplicationPrefix)) {
                var modelDifferenceObjectInfo = GetModelDifferenceObjectInfo(modelApplicationPrefix, loadedModelDifferenceObjectInfos, resourse.Key, model);
                foreach (var xml in resourse.Value){
                    modelXmlReader.ReadFromString(modelDifferenceObjectInfo.Model, "", xml);
                }
                modelDifferenceObjectInfo.ModelDifferenceObject.CreateAspects(modelDifferenceObjectInfo.Model);
            }
        }
        ModelDifferenceObject CreateDifferenceObject(string resourceName, string prefix)
        {
            ModelDifferenceObject modelDifferenceObject;
            if (prefix == XpoModelDictionaryDifferenceStore.ModelApplicationPrefix)
                modelDifferenceObject = new ModelDifferenceObject(_objectSpace.Session);
            else
            {
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

        ModelDifferenceObject FindDifferenceObject(string resourceName, string prefix)
        {
            if (prefix == XpoModelDictionaryDifferenceStore.ModelApplicationPrefix)
                return _xpoModelDictionaryDifferenceStore.GetActiveDifferenceObject(resourceName);
            return _objectSpace.Session.FindObject<RoleModelDifferenceObject>(o => o.Name == resourceName);
        }

        ModelDifferenceObjectInfo GetModelDifferenceObjectInfo(string prefix, Dictionary<string, ModelDifferenceObjectInfo> loadedModelDifferenceObjectInfos, string resourceName, ModelApplicationBase model)
        {
            ModelDifferenceObject activeDifferenceObject;
            ModelApplicationBase modelApplicationBase;
            if (!loadedModelDifferenceObjectInfos.ContainsKey(resourceName))
            {
                activeDifferenceObject = FindDifferenceObject(resourceName, prefix) ??
                                         CreateDifferenceObject(resourceName, prefix);
                modelApplicationBase = activeDifferenceObject.GetModel(model);
            }
            else
            {
                var loadedModelDifferenceObjectInfo = loadedModelDifferenceObjectInfos[resourceName];
                activeDifferenceObject = loadedModelDifferenceObjectInfo.ModelDifferenceObject;
                modelApplicationBase = loadedModelDifferenceObjectInfo.Model;
            }
            return new ModelDifferenceObjectInfo(activeDifferenceObject, modelApplicationBase);
        }

    }
}