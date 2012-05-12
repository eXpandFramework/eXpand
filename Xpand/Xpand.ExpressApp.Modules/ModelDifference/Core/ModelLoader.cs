using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Core;

namespace Xpand.ExpressApp.ModelDifference.Core {
    public class ApplicationBuilder {
        string _assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        Func<string, ITypesInfo> _buildTypesInfoSystem = BuildTypesInfoSystem(true);
        string _moduleName;

        ApplicationBuilder() {
        }

        public static ApplicationBuilder Create() {
            return new ApplicationBuilder();
        }

        static Func<string, ITypesInfo> BuildTypesInfoSystem(bool tryToUseCurrentTypesInfo) {
            return moduleName => TypesInfoBuilder.Create()
                                     .FromModule(moduleName)
                                     .Build(tryToUseCurrentTypesInfo);
        }

        public ApplicationBuilder FromAssembliesPath(string path) {
            _assemblyPath = path;
            return this;
        }
        public ApplicationBuilder UsingTypesInfo(Func<string, ITypesInfo> buildTypesInfoSystem) {
            _buildTypesInfoSystem = buildTypesInfoSystem;
            return this;
        }

        public ApplicationBuilder FromModule(string moduleName) {
            _moduleName = moduleName;
            return this;
        }

        public XafApplication Build() {

            try {
                var typesInfo = _buildTypesInfoSystem.Invoke(_moduleName);
                ReflectionHelper.AddResolvePath(_assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(_moduleName), _assemblyPath);
                var assemblyInfo = typesInfo.FindAssemblyInfo(assembly);
                typesInfo.LoadTypes(assembly);
                var findTypeInfo = typesInfo.FindTypeInfo(typeof(XafApplication));
                var findTypeDescendants = ReflectionHelper.FindTypeDescendants(assemblyInfo, findTypeInfo, false);
                var instance = SecuritySystem.Instance;
                var xafApplication = ((XafApplication)Enumerator.GetFirst(findTypeDescendants).CreateInstance(new object[0]));
                SecuritySystem.SetInstance(instance);
                if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                    ((ISupportFullConnectionString)xafApplication).ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
                return xafApplication;
            } finally {
                ReflectionHelper.RemoveResolvePath(_assemblyPath);
            }
        }
    }

    public class TypesInfoBuilder {
        string _moduleName;

        public static TypesInfoBuilder Create() {
            return new TypesInfoBuilder();
        }

        public TypesInfoBuilder FromModule(string moduleName) {
            _moduleName = moduleName;
            return this;
        }

        public ITypesInfo Build(bool tryToUseCurrentTypesInfo) {
            return tryToUseCurrentTypesInfo
                       ? (UseCurrentTypesInfo() ? XafTypesInfo.Instance : GetTypesInfo())
                       : GetTypesInfo();
        }

        bool UseCurrentTypesInfo() {
            return _moduleName == XpandModuleBase.ManifestModuleName;
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
            public XpoTypeInfoSource Source { get; set; }
        }

    }

    internal class ModelBuilder {
        readonly string _assembliesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        XafApplication _application;
        ITypesInfo _typesInfo;
        string _moduleName;

        ModelBuilder() {
        }
        private IEnumerable<string> GetAspects(string configFileName) {
            if (!string.IsNullOrEmpty(configFileName) && configFileName.EndsWith(".config")) {
                var exeConfigurationFileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFileName };
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
                KeyValueConfigurationElement languagesElement = configuration.AppSettings.Settings["Languages"];
                if (languagesElement != null) {
                    string languages = languagesElement.Value;
                    return languages.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            return null;
        }

        public static ModelBuilder Create() {
            return new ModelBuilder();
        }
        string GetConfigPath() {
            string path = Path.Combine(_assembliesPath, _moduleName);
            string config = path + ".config";
            if (File.Exists(_assembliesPath + "web.config"))
                config = Path.Combine(_assembliesPath, "web.config");
            return config;
        }
        private string[] GetModulesFromConfig(XafApplication application) {
            Configuration config;
            if (application is IWinApplication) {
                config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _moduleName);
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
        ModelApplicationBase BuildModel(XafApplication application, string configFileName, ApplicationModulesManager applicationModulesManager) {
            var modelsManager = new ApplicationModelsManager(applicationModulesManager.Modules, applicationModulesManager.ControllersManager, applicationModulesManager.DomainComponents, application.ResourcesExportedToModel, GetAspects(configFileName));
            var assemblyFile = application.GetType().GetMethod("GetModelAssemblyFilePath", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(application, null) as string;
            modelsManager.GetType().GetProperty("ModelAssemblyFile", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(modelsManager, assemblyFile, null);
            var modelApplication = (ModelApplicationBase)modelsManager.CreateModel(modelsManager.CreateApplicationCreator(), null, false);
            var modelApplicationBase = modelApplication.CreatorInstance.CreateModelApplication();
            modelApplicationBase.Id = "After Setup";
            modelApplication.AddLayer(modelApplicationBase);
            return modelApplication;
        }

        ApplicationModulesManager CreateModulesManager(XafApplication application, string configFileName, string assembliesPath, ITypesInfo typesInfo) {
            if (!string.IsNullOrEmpty(configFileName)) {
                bool isWebApplicationModel = String.Compare(Path.GetFileNameWithoutExtension(configFileName), "web", StringComparison.OrdinalIgnoreCase) == 0;
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
                    result.AddModuleFromAssemblies(GetModulesFromConfig(application));
                }
                var info = typesInfo as TypesInfoBuilder.TypesInfo;
                if (info != null) XpandModuleBase.Dictiorary = (info).Source.XPDictionary;
                XpandModuleBase.TypesInfo = typesInfo;
                result.Load(typesInfo, typesInfo != XafTypesInfo.Instance);
                return result;
            } finally {
                XpandModuleBase.Dictiorary = XpoTypesInfoHelper.GetXpoTypeInfoSource().XPDictionary;
                XpandModuleBase.TypesInfo = XafTypesInfo.Instance;
                ReflectionHelper.RemoveResolvePath(assembliesPath);
            }

        }

        public ModelBuilder WithApplication(XafApplication xafApplication) {
            _application = xafApplication;
            return this;
        }

        public ModelApplicationBase Build() {
            string config = GetConfigPath();
            var modulesManager = CreateModulesManager(_application, config, _assembliesPath, _typesInfo);
            return BuildModel(_application, config, modulesManager);
        }

        public ModelBuilder UsingTypesInfo(ITypesInfo typesInfo) {
            _typesInfo = typesInfo;
            return this;
        }

        public ModelBuilder FromModule(string moduleName) {
            _moduleName = moduleName;
            return this;
        }
    }
    public class ModelLoader {
        readonly string _moduleName;

        public ModelLoader(string moduleName) {
            _moduleName = moduleName;
        }

        public ModelApplicationBase GetMasterModel(bool tryToUseCurrentTypesInfo) {
            var typesInfo = TypesInfoBuilder.Create()
                .FromModule(_moduleName)
                .Build(tryToUseCurrentTypesInfo);
            var xafApplication = ApplicationBuilder.Create().
                UsingTypesInfo(s => typesInfo).
                FromModule(_moduleName).
                Build();
            XpandModuleBase.DisposeManagers();
            ModelApplicationBase modelApplicationBase;
            try {
                modelApplicationBase = ModelBuilder.Create()
                    .UsingTypesInfo(typesInfo)
                    .FromModule(_moduleName)
                    .WithApplication(xafApplication)
                    .Build();
            } catch (CompilerErrorException e) {
                Tracing.Tracer.LogSeparator("CompilerErrorException");
                Tracing.Tracer.LogError(e);
                Tracing.Tracer.LogValue("Source Code", e.SourceCode);
                throw;
            }
            XpandModuleBase.ReStoreManagers();
            return modelApplicationBase;
        }

        public ModelApplicationBase GetLayer(Type modelApplicationFromStreamStoreBaseType, bool tryToUseCurrentTypesInfo) {
            var masterModel = GetMasterModel(tryToUseCurrentTypesInfo);
            var layer = masterModel.CreatorInstance.CreateModelApplication();

            masterModel.AddLayerBeforeLast(layer);
            var storeBase = (ModelApplicationFromStreamStoreBase)ReflectionHelper.CreateObject(modelApplicationFromStreamStoreBaseType);
            storeBase.Load(layer);
            return layer;
        }
    }
}
