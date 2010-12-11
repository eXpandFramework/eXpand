using System;
using System.IO;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference.Core {
    
    public class ModelLoader {
        readonly string _executableName;

        public ModelLoader(string executableName) {
            _executableName = executableName;
        }

        public ModelApplicationBase GetMasterModel() {
            ITypesInfo typesInfo = GetTypesInfo(_executableName);
            var xafApplication = GetApplication(_executableName, typesInfo);
            var assembliesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string path = Path.Combine(assembliesPath, _executableName);
            string config = path + ".config";
            if (File.Exists(assembliesPath + "web.config"))
                config = Path.Combine(assembliesPath, "web.config");
            var modulesManager = CreateModulesManager(xafApplication, config, assembliesPath,  typesInfo);
            return GetModelApplication(modulesManager);
        }

        ApplicationModulesManager CreateModulesManager(XafApplication application, string configFileName, string assembliesPath, ITypesInfo typesInfo) {
            if (!string.IsNullOrEmpty(configFileName)) {
                bool isWebApplicationModel = string.Compare(Path.GetFileNameWithoutExtension(configFileName), "web", true) == 0;
                if (string.IsNullOrEmpty(assembliesPath)) {
                    assembliesPath = Path.GetDirectoryName(configFileName);
                    if (isWebApplicationModel) {
                        assembliesPath = Path.Combine(assembliesPath + "", "Bin");
                    }
                }
            }
            ReflectionHelper.AddResolvePath(assembliesPath);
            try {
                var result = new ApplicationModulesManager(new ControllersManager(), assembliesPath);
                if (application != null) {
                    foreach (ModuleBase module in application.Modules) {
                        result.AddModule(module);
                    }
                    result.Security = application.Security;
                }
                if (!string.IsNullOrEmpty(configFileName)) {
                    result.AddModuleFromAssemblies(ReadModulesFromConfig(configFileName).Split(';'));
                }
                if (typesInfo is TypesInfo)
                    XpandModuleBase.Dictiorary = ((TypesInfo) typesInfo).Source.XPDictionary;
                
                result.Load(typesInfo);
                return result;
            } finally {
                XpandModuleBase.Dictiorary = XafTypesInfo.XpoTypeInfoSource.XPDictionary;
                ReflectionHelper.RemoveResolvePath(assembliesPath);
            }

        }

        private string ReadModulesFromConfig(string configFileName) {
            if (!File.Exists(configFileName)) {
                throw new ArgumentException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ConfigFileDoesNotExists, configFileName));
            }
            return string.Empty;
        }

        ITypesInfo GetTypesInfo(string executableName) {
            return executableName == Assembly.GetAssembly(XpandModuleBase.Application.GetType()).ManifestModule.Name
                       ? XafTypesInfo.Instance
                       : GetTypesInfo();
        }

        TypesInfo GetTypesInfo() {
            var typesInfo = new TypesInfo();
            var xpoSource = new XpoTypeInfoSource(typesInfo);
            typesInfo.Source = xpoSource;
            typesInfo.AddSource(new ReflectionTypeInfoSource());
            typesInfo.AddSource(xpoSource);
            typesInfo.AddSource(new DynamicTypeInfoSource());
            typesInfo.SetRedirectStrategy((@from, info) => xpoSource.GetFirstRegisteredTypeForEntity(from) ?? from);
            XpandModuleBase.Dictiorary = xpoSource.XPDictionary;
            return typesInfo;
        }

        public class TypesInfo : DevExpress.ExpressApp.DC.TypesInfo {

            public DevExpress.ExpressApp.DC.Xpo.XpoTypeInfoSource Source { get; set; }

                        
        }

        private XafApplication GetApplication(string executableName, ITypesInfo typesInfo) {
            string assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try {
                ReflectionHelper.AddResolvePath(assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(executableName), assemblyPath);
                var assemblyInfo = typesInfo.FindAssemblyInfo(assembly);
                typesInfo.LoadTypes(assembly);
                var findTypeInfo = typesInfo.FindTypeInfo(typeof(XafApplication));
                var findTypeDescendants = ReflectionHelper.FindTypeDescendants(assemblyInfo, findTypeInfo, false);
                return Enumerator.GetFirst(findTypeDescendants).CreateInstance(new object[0]) as XafApplication;
            } finally {
                ReflectionHelper.RemoveResolvePath(assemblyPath);
            }
        }
        public ModelApplicationBase GetLayer(Type modelApplicationFromStreamStoreBaseType) {
            var masterModel = GetMasterModel();
            var layer = masterModel.CreatorInstance.CreateModelApplication();

            masterModel.AddLayerBeforeLast(layer);
            var storeBase = (ModelApplicationFromStreamStoreBase)ReflectionHelper.CreateObject(modelApplicationFromStreamStoreBaseType);
            storeBase.Load(layer);
            return layer;
        }

        ModelApplicationBase GetModelApplication(ApplicationModulesManager applicationModulesManager) {
            var modelsManager = new ApplicationModelsManager(applicationModulesManager.Modules, applicationModulesManager.ControllersManager, applicationModulesManager.DomainComponents);
            var modelApplication = (ModelApplicationBase)modelsManager.CreateModel(modelsManager.CreateApplicationCreator(), null, false);
            var modelApplicationBase = modelApplication.CreatorInstance.CreateModelApplication();
            modelApplicationBase.Id = "After Setup";
            modelApplication.AddLayer(modelApplicationBase);
            return modelApplication;
        }
    }
}
