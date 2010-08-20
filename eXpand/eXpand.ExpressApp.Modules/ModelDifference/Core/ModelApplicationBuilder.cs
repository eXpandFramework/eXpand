using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using ModelApplicationBaseExtensions = eXpand.ExpressApp.Core.ModelApplicationBaseExtensions;


namespace eXpand.ExpressApp.ModelDifference.Core {
    public class XpoTypeInfoSource : DevExpress.ExpressApp.DC.Xpo.XpoTypeInfoSource
    {
        public XpoTypeInfoSource(TypesInfo typesInfo) : base(typesInfo) {
        }

        public new Type GetFirstRegisteredTypeForEntity(Type from)
        {
            return base.GetFirstRegisteredTypeForEntity(from);
        }
    }
    public class ModelApplicationBuilder {
        readonly string _executableName;
        ICurrentAspectProvider _oldAspectProvider;

        public ModelApplicationBuilder(string executableName) {
            _executableName = executableName;
        }
        public ApplicationModulesManager CreateApplicationModelManager(XafApplication application, string configFileName, string assembliesPath, ITypesInfo typesInfo)
        {
            if (!string.IsNullOrEmpty(configFileName)) {
                bool isWebApplicationModel =
                    string.Compare(Path.GetFileNameWithoutExtension(configFileName), "web", true) == 0;
                if (string.IsNullOrEmpty(assembliesPath)) {
                    assembliesPath = Path.GetDirectoryName(configFileName);
                    if (isWebApplicationModel) {
                        assembliesPath = Path.Combine(assembliesPath, "Bin");
                    }
                }
            }
            ReflectionHelper.AddResolvePath(assembliesPath);
            try {
                var result = new ExpressApp.Core.ApplicationModulesManager(new ControllersManager(), assembliesPath,typesInfo);
                if (application != null) {
                    foreach (DevExpress.ExpressApp.ModuleBase module in application.Modules) {
                        result.AddModule(module);
                    }
                    result.Security = application.Security;
                }
                return result;
            }
            finally {
                ReflectionHelper.RemoveResolvePath(assembliesPath);
            }
        }

        public ModelApplicationBase GetMasterModel() {
            if (_executableName != Assembly.GetAssembly(ModuleBase.Application.GetType()).ManifestModule.Name)
                return GetExternalMasterModel();

            var masterModel = (ModelApplicationBase)ModuleBase.Application.Model;
            _oldAspectProvider = masterModel.CurrentAspectProvider;
            masterModel.CurrentAspectProvider = new CurrentAspectProvider(_oldAspectProvider.CurrentAspect);
            return masterModel;
        }

        ModelApplicationBase GetExternalMasterModel() {
            var typesInfo = new TypesInfo();
            typesInfo.AddSource(new ReflectionTypeInfoSource());
            var xpoSource = new XpoTypeInfoSource(typesInfo);
            typesInfo.AddSource(xpoSource);
            typesInfo.AddSource(new DynamicTypeInfoSource());
            typesInfo.SetRedirectStrategy((@from, info) => xpoSource.GetFirstRegisteredTypeForEntity(from) ?? from);   
            var application = GetApplication(_executableName, typesInfo);

            var modulesManager = CreateApplicationModelManager(application,string.Empty,
                                                               AppDomain.CurrentDomain.SetupInformation.ApplicationBase,typesInfo);

            ReadModulesFromConfig(modulesManager, application);

            modulesManager.Load(typesInfo);

            var modelsManager = new ApplicationModelsManager(
                modulesManager.Modules,
                modulesManager.ControllersManager,
                modulesManager.DomainComponents);
            var modelApplicationCreator = ModuleBase.ModelApplicationCreator;
            ModuleBase.ModelApplicationCreator = null;
            var modelApplication = modelsManager.CreateModelApplication();
            AddAfterSetupLayer(modelApplication);
            ModuleBase.ModelApplicationCreator=modelApplicationCreator;
            application.Dispose();
            return (ModelApplicationBase) modelApplication;
        }

        void AddAfterSetupLayer(IModelApplication modelApplication) {
            var modelApplicationBase = ((ModelApplicationBase) modelApplication);
            ModelApplicationBase afterSetup = modelApplicationBase.CreatorInstance.CreateModelApplication();
            afterSetup.Id = "After Setup";
            modelApplicationBase.AddLayer(afterSetup);
        }

        private XafApplication GetApplication(string executableName, TypesInfo typesInfo)
        {
            string assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                ReflectionHelper.AddResolvePath(assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(executableName), assemblyPath);
                var assemblyInfo = typesInfo.FindAssemblyInfo(assembly);
                ((ITypesInfo) typesInfo).LoadTypes(assembly);
                var findTypeInfo = typesInfo.FindTypeInfo(typeof(XafApplication));
                var findTypeDescendants = ReflectionHelper.FindTypeDescendants(assemblyInfo, findTypeInfo, false);
                return Enumerator.GetFirst(findTypeDescendants).CreateInstance(new object[0]) as XafApplication;
            }
            finally
            {
                ReflectionHelper.RemoveResolvePath(assemblyPath);
            }
        }

        private void ReadModulesFromConfig(ApplicationModulesManager manager, XafApplication application)
        {
            Configuration config;
            if (application is IWinApplication)
            {
                config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + _executableName);
            }
            else
            {
                var mapping = new WebConfigurationFileMap();
                mapping.VirtualDirectories.Add("/Dummy", new VirtualDirectoryMapping(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, true));
                config = WebConfigurationManager.OpenMappedWebConfiguration(mapping, "/Dummy");
            }

            if (config.AppSettings.Settings["Modules"] != null)
            {
                manager.AddModuleFromAssemblies(config.AppSettings.Settings["Modules"].Value.Split(';'));
            }
        }


        public ModelApplicationBase GetLayer(Type modelApplicationFromStreamStoreBaseType) {
            var masterModel = GetMasterModel();
            var layer = masterModel.CreatorInstance.CreateModelApplication();

            ModelApplicationBaseExtensions.AddLayerBeforeLast(masterModel, layer);
            var storeBase =(ModelApplicationFromStreamStoreBase)ReflectionHelper.CreateObject(modelApplicationFromStreamStoreBaseType);
            storeBase.Load(layer);
            return layer;
        }

        public void ResetModel() {
            while (ModelDifferenceModule.MasterModel.LastLayer.Id != "UserDiff" && ModelDifferenceModule.MasterModel.LastLayer.Id != "After Setup"){
                ModelDifferenceModule.MasterModel.RemoveLayer(ModelDifferenceModule.MasterModel.LastLayer);
            }
            if (_oldAspectProvider != null)
                ModelDifferenceModule.MasterModel.CurrentAspectProvider=_oldAspectProvider;
            ModelDifferenceModule.MasterModel = null;
        }
    }
}