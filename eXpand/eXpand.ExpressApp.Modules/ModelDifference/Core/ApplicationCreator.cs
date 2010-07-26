using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;

namespace eXpand.ExpressApp.ModelDifference.Core {
    public  class ApplicationCreator
    {




        public ModelApplicationBase CreateModelApplication(string executableName)
        {
            return GetMasterModel(executableName);
        }
        private ModelApplicationBase GetMasterModel(string executableName)
        {
            XafApplication xafApplication = GetApplication(executableName);
            var modulesManager = new DesignerModelFactory().CreateApplicationModelManager(
                xafApplication, 
                string.Empty,
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase);

//            ReadModulesFromConfig(modulesManager, _xafApplication,executableName);

            modulesManager.Load();

            var modelsManager = new ApplicationModelsManager(
                modulesManager.Modules,
                modulesManager.ControllersManager,
                modulesManager.DomainComponents);

            var modelApplicationBase = modelsManager.CreateModelApplication() as ModelApplicationBase;
            xafApplication.Exit();
            return modelApplicationBase;

        }
//        private static void ReadModulesFromConfig(ApplicationModulesManager manager, XafApplication application, string executableName)
//        {
//            Configuration config;
//            if (application is IWinApplication)
//            {
//                config = ConfigurationManager.OpenExeConfiguration(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + executableName);
//            }
//            else
//            {
//                var mapping = new WebConfigurationFileMap();
//                mapping.VirtualDirectories.Add("/Dummy", new VirtualDirectoryMapping(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, true));
//                config = WebConfigurationManager.OpenMappedWebConfiguration(mapping, "/Dummy");
//            }
//
//            if (config.AppSettings.Settings["Modules"] != null)
//            {
//                manager.AddModuleFromAssemblies(config.AppSettings.Settings["Modules"].Value.Split(';'));
//            }
//        }

        private static XafApplication GetApplication(string executableName)
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

    }
}