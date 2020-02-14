using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.ModelEditor.ModelDifference;


namespace Xpand.ExpressApp.ModelEditor {
    public class ModelControllerBuilder {
        public ModelEditorViewController GetController(PathInfo pathInfo) {
            var storePath = Path.GetDirectoryName(pathInfo.LocalPath);
            var fileModelStore = new FileModelStore(storePath, Path.GetFileNameWithoutExtension(pathInfo.LocalPath));
            var applicationModulesManager = GetApplicationModulesManager(pathInfo);
            Tracing.Tracer.LogText("applicationModulesManager");
            var modelApplication = GetModelApplication(applicationModulesManager, pathInfo, fileModelStore);
            return GetController(fileModelStore, modelApplication);
        }
        ModelEditorViewController GetController(FileModelStore fileModelStore, ModelApplicationBase modelApplication) {
            return new ModelEditorViewController((IModelApplication)modelApplication, fileModelStore);
        }

        ModelApplicationBase GetModelApplication(ApplicationModulesManager applicationModulesManager, PathInfo pathInfo, FileModelStore fileModelStore) {
            var modelApplication = ModelApplicationHelper.CreateModel(XafTypesInfo.Instance, applicationModulesManager.DomainComponents, applicationModulesManager.Modules, applicationModulesManager.ControllersManager, Type.EmptyTypes, fileModelStore.GetAspects(), null, null);
            AddLayers(modelApplication, applicationModulesManager, pathInfo);
            Tracing.Tracer.LogText("AddLayers");
            ModelApplicationBase lastLayer = modelApplication.CreatorInstance.CreateModelApplication();
            fileModelStore.Load(lastLayer);
            ModelApplicationHelper.AddLayer(modelApplication, lastLayer);
            return modelApplication;
        }

        ApplicationModulesManager GetApplicationModulesManager(PathInfo pathInfo) {
            string assemblyPath = Path.GetDirectoryName(pathInfo.AssemblyPath);
            var designerModelFactory = new DesignerModelFactory();
            ReflectionHelper.Reset();
            XafTypesInfo.HardReset();
            XpoTypesInfoHelper.ForceInitialize();
            if (pathInfo.IsApplicationModel) {
                _currentDomainOnAssemblyResolvePathInfo = pathInfo;
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainOnAssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
                var applicationInstance = Activator.CreateInstance(Assembly.Load(pathInfo.AssemblyPath).GetTypes().First(type => typeof(XafApplication).IsAssignableFrom(type)));
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= CurrentDomainOnAssemblyResolve;
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
                _currentDomainOnAssemblyResolvePathInfo = null;

                var configFileName = applicationInstance is WinApplication? pathInfo.AssemblyPath+".config":Path.Combine(pathInfo.FullPath,"web.config");
                return designerModelFactory.CreateModulesManager((XafApplication) applicationInstance, configFileName,Path.GetDirectoryName(pathInfo.AssemblyPath));
            }
            var moduleFromFile = designerModelFactory.CreateModuleFromFile(pathInfo.AssemblyPath, assemblyPath);
            return designerModelFactory.CreateModulesManager(moduleFromFile, pathInfo.AssemblyPath);
        }

        private PathInfo _currentDomainOnAssemblyResolvePathInfo;

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args) {
            if (File.Exists(args.Name)) {
                return GetLoadedAssembly(args.Name) ?? Assembly.LoadFrom(args.Name);
            }

            var dllName = args.Name.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).First().Trim();
            var localAssemblyName =
                Path.Combine(Directory.GetParent(_currentDomainOnAssemblyResolvePathInfo.AssemblyPath).FullName,
                    $"{dllName}.dll");
            if (File.Exists(localAssemblyName)) {
                return GetLoadedAssembly(localAssemblyName) ?? Assembly.LoadFrom(localAssemblyName);
            }

            return null;
        }

        private static Assembly GetLoadedAssembly(string assemblyPath) {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(s => s.GetName() == assemblyName);
        }

        void AddLayers(ModelApplicationBase modelApplication, ApplicationModulesManager applicationModulesManager, PathInfo pathInfo) {
            var resourceModelCollector = new ResourceModelCollector();
            var resourceInfos = resourceModelCollector.Collect(applicationModulesManager.Modules.Select(@base => @base.GetType().Assembly), null).Where(pair => !MatchLastLayer(pair, pathInfo));
            AddLayersCore(resourceInfos, modelApplication);
            ModelApplicationBase lastLayer = modelApplication.CreatorInstance.CreateModelApplication();
            ModelApplicationHelper.AddLayer(modelApplication, lastLayer);
        }

        bool MatchLastLayer(KeyValuePair<string, ResourceInfo> pair, PathInfo pathInfo) {
            var name = pair.Key.EndsWith(ModelStoreBase.ModelDiffDefaultName) ? ModelStoreBase.ModelDiffDefaultName : pair.Key.Substring(pair.Key.LastIndexOf(".", StringComparison.Ordinal) + 1);
            bool nameMatch = (name.EndsWith(Path.GetFileNameWithoutExtension(pathInfo.LocalPath) + ""));
            bool assemblyMatch = Path.GetFileNameWithoutExtension(pathInfo.AssemblyPath) == pair.Value.AssemblyName;
            return nameMatch && assemblyMatch;
        }

        void AddLayersCore(IEnumerable<KeyValuePair<string, ResourceInfo>> layers, ModelApplicationBase modelApplication) {
            IEnumerable<KeyValuePair<string, ResourceInfo>> keyValuePairs = layers;
            foreach (var pair in keyValuePairs) {
                ModelApplicationBase layer = modelApplication.CreatorInstance.CreateModelApplication();
                layer.Id = pair.Key;
                ModelApplicationHelper.AddLayer(modelApplication, layer);
                var modelXmlReader = new ModelXmlReader();
                foreach (var aspectInfo in pair.Value.AspectInfos) {
                    modelXmlReader.ReadFromString(layer, aspectInfo.AspectName, aspectInfo.Xml);
                }
            }
        }
    }
}