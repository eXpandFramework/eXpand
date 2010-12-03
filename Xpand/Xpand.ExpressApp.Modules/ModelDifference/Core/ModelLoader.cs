using System;
using System.IO;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
            var designerModelFactory = new DesignerModelFactory();
            var xafApplication = GetApplication(_executableName, XafTypesInfo.Instance);
            var assembliesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var modulesManager = designerModelFactory.CreateModulesManager(xafApplication, Path.Combine(assembliesPath, _executableName) + ".config", assembliesPath);
            return GetModelApplication(modulesManager);
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
