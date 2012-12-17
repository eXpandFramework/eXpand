using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.PersistentMetaData;


namespace Xpand.ExpressApp.ModelDifference.Core {
    [Obsolete("User ModelLoader", true)]
    public class ModelApplicationBuilder {
        readonly string _executableName;


        public ModelApplicationBuilder(string executableName) {
            _executableName = executableName;
        }
        public ApplicationModulesManager CreateApplicationModulesManager(XafApplication application, string configFileName, string assembliesPath, ITypesInfo typesInfo) {
            if (!string.IsNullOrEmpty(configFileName)) {
                bool isWebApplicationModel =
                    String.Compare(Path.GetFileNameWithoutExtension(configFileName), "web", StringComparison.OrdinalIgnoreCase) == 0;
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
                foreach (ModuleBase module in application.Modules) {
                    result.AddModule(module);
                }
                result.Security = application.Security;
                if (GetModulesFromConfig(application) != null) {
                    result.AddModuleFromAssemblies(GetModulesFromConfig(application));
                }
                return result;
            } finally {
                ReflectionHelper.RemoveResolvePath(assembliesPath);
            }
        }

        public ModelApplicationBase GetMasterModel() {
            TypesInfo typesInfo = GetTypesInfo();
            using (var application = GetApplication(_executableName, typesInfo)) {
                ApplicationModulesManager modulesManager = GetModulesManager(typesInfo, application);
                var masterModel = GetModelApplication(application, modulesManager);
                return masterModel;
            }
        }

        TypesInfo GetTypesInfo() {
            var typesInfo = new TypesInfo();
            typesInfo.AddEntityStore(new NonPersistentEntityStore(typesInfo));
            var xpoSource = new XpoTypeInfoSource(typesInfo);
            typesInfo.AddEntityStore(xpoSource);
            XpandModuleBase.Dictiorary = xpoSource.XPDictionary;
            return typesInfo;
        }

        ModelApplicationBase GetModelApplication(XafApplication application, ApplicationModulesManager modulesManager) {
            var modelApplicationCreator = XpandModuleBase.ModelApplicationCreator;
            XpandModuleBase.ModelApplicationCreator = null;
            var modelApplication = new DesignerModelFactory().CreateApplicationModel(application, modulesManager, null, null);
            // ReSharper disable SuspiciousTypeConversion.Global
            var modelApplicationBase = (ModelApplicationBase)modelApplication;
            // ReSharper restore SuspiciousTypeConversion.Global
            AddAfterSetupLayer(modelApplicationBase);
            XpandModuleBase.ModelApplicationCreator = modelApplicationCreator;
            return modelApplicationBase;
        }
        public class TypesInfo : DevExpress.ExpressApp.DC.TypesInfo {

            public XpoTypeInfoSource Source { get; set; }


        }

        ApplicationModulesManager GetModulesManager(TypesInfo typesInfo, XafApplication application) {
            var modulesManager = CreateApplicationModulesManager(application, string.Empty,
                                                                 AppDomain.CurrentDomain.SetupInformation.ApplicationBase, typesInfo);
            try {
                XpandModuleBase.Dictiorary = typesInfo.Source.XPDictionary;
                XpandModuleBase.TypesInfo = typesInfo;
                modulesManager.Load(typesInfo);
            } finally {
                XpandModuleBase.Dictiorary = ((XpoTypeInfoSource)XafTypesInfo.XpoTypeInfoSource).XPDictionary;
                XpandModuleBase.TypesInfo = XafTypesInfo.Instance;
            }
            return modulesManager;
        }

        void AddAfterSetupLayer(ModelApplicationBase modelApplication) {
            var modelApplicationBase = modelApplication;
            ModelApplicationBase afterSetup = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetup.Id = "After Setup";
            modelApplicationBase.AddLayer(afterSetup);
        }

        private XafApplication GetApplication(string executableName, TypesInfo typesInfo) {
            string assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try {
                ReflectionHelper.AddResolvePath(assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(executableName), assemblyPath);
                var assemblyInfo = typesInfo.FindAssemblyInfo(assembly);
                ((ITypesInfo)typesInfo).LoadTypes(assembly);
                var findTypeInfo = typesInfo.FindTypeInfo(typeof(XafApplication));
                var findTypeDescendants = ReflectionHelper.FindTypeDescendants(assemblyInfo, findTypeInfo, false);
                return Enumerator.GetFirst(findTypeDescendants).CreateInstance(new object[0]) as XafApplication;
            } finally {
                ReflectionHelper.RemoveResolvePath(assemblyPath);
            }
        }

        private string[] GetModulesFromConfig(XafApplication application) {
            Configuration config;
            if (application is IWinApplication) {
                config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _executableName);
            } else {
                var mapping = new WebConfigurationFileMap();
                mapping.VirtualDirectories.Add("/Dummy", new VirtualDirectoryMapping(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, true));
                config = WebConfigurationManager.OpenMappedWebConfiguration(mapping, "/Dummy");
            }

            if (config.AppSettings.Settings["Modules"] != null) {
                return config.AppSettings.Settings["Modules"].Value.Split(';');
            }

            return null;
        }

        public ModelApplicationBase GetLayer(Type modelApplicationFromStreamStoreBaseType) {
            var masterModel = GetMasterModel();
            var layer = masterModel.CreatorInstance.CreateModelApplication();

            masterModel.AddLayerBeforeLast(layer);
            var storeBase = (ModelApplicationFromStreamStoreBase)ReflectionHelper.CreateObject(modelApplicationFromStreamStoreBaseType);
            storeBase.Load(layer);
            return layer;
        }
    }
}