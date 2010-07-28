using System;
using System.Configuration;
using System.IO;
using System.Web.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.Core {
    public class ModelApplicationBuilder {
        readonly string _executableName;

        public ModelApplicationBuilder(string executableName) {
            _executableName = executableName;
        }

        public ModelApplicationBase GetMasterModel()
        {

            var application = GetApplication(_executableName);

            var modulesManager = new DesignerModelFactory().CreateApplicationModelManager(
                application,
                string.Empty,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

            ReadModulesFromConfig(modulesManager, application);

            modulesManager.Load();

            var modelsManager = new ApplicationModelsManager(
                modulesManager.Modules,
                modulesManager.ControllersManager,
                modulesManager.DomainComponents);

            var modelApplicationCreator = ModuleBase.ModelApplicationCreator;
            ModuleBase.ModelApplicationCreator = null;
            var modelApplication = modelsManager.CreateModelApplication();
            ModuleBase.ModelApplicationCreator=modelApplicationCreator;
            return (ModelApplicationBase) modelApplication;
        }

        private XafApplication GetApplication(string executableName)
        {
            string assemblyPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            try
            {
                ReflectionHelper.AddResolvePath(assemblyPath);
                var assembly = ReflectionHelper.GetAssembly(Path.GetFileNameWithoutExtension(executableName), assemblyPath);
                var assemblyInfo = XafTypesInfo.Instance.FindAssemblyInfo(assembly);
                XafTypesInfo.Instance.LoadTypes(assembly);
                return Enumerator.GetFirst(ReflectionHelper.FindTypeDescendants(assemblyInfo, XafTypesInfo.Instance.FindTypeInfo(typeof(XafApplication)), false)).CreateInstance(new object[0]) as XafApplication;
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
            masterModel.AddLayer(layer);
            var storeBase =(ModelApplicationFromStreamStoreBase)ReflectionHelper.CreateObject(modelApplicationFromStreamStoreBaseType);
            storeBase.Load(layer);
            return layer;
        }
    }
}