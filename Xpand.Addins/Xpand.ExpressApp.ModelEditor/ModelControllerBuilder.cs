using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core.ModelEditor;
using Xpand.Persistent.Base.ModelDifference;


namespace Xpand.ExpressApp.ModelEditor {
    public class ModelControllerBuilder {
        public ModelEditorViewController GetController(PathInfo pathInfo) {
            var storePath = Path.GetDirectoryName(pathInfo.LocalPath);
            var fileModelStore = new FileModelStore(storePath, Path.GetFileNameWithoutExtension(pathInfo.LocalPath));
            var applicationModulesManager = GetApplicationModulesManager(pathInfo);
            var modelApplication = GetModelApplication(applicationModulesManager, pathInfo, fileModelStore);
            return GetController(fileModelStore, modelApplication);
        }
        ModelEditorViewController GetController(FileModelStore fileModelStore, ModelApplicationBase modelApplication) {
            fileModelStore.Load(modelApplication.LastLayer);
            return new ModelEditorViewController((IModelApplication)modelApplication, fileModelStore);
        }

        ModelApplicationBase GetModelApplication(ApplicationModulesManager applicationModulesManager, PathInfo pathInfo, FileModelStore fileModelStore) {
            var modelsManager = new ApplicationModelsManager(applicationModulesManager.Modules, applicationModulesManager.ControllersManager, applicationModulesManager.DomainComponents, null, fileModelStore.GetAspects());
            var modelApplication = (ModelApplicationBase)modelsManager.CreateModel(modelsManager.CreateApplicationCreator(), null, false);
            AddLayers(modelApplication, applicationModulesManager, pathInfo);
            return modelApplication;
        }

        ApplicationModulesManager GetApplicationModulesManager(PathInfo pathInfo) {
            string assemblyPath = Path.GetDirectoryName(pathInfo.AssemblyPath);
            var designerModelFactory = new DesignerModelFactory();
            var moduleFromFile = designerModelFactory.CreateModuleFromFile(pathInfo.AssemblyPath, assemblyPath);
            return designerModelFactory.CreateModulesManager(moduleFromFile, pathInfo.AssemblyPath);
        }


        void AddLayers(ModelApplicationBase modelApplication, ApplicationModulesManager applicationModulesManager, PathInfo pathInfo) {
            var resourceModelCollector = new ResourceModelCollector();
            var dictionary = resourceModelCollector.Collect(applicationModulesManager.Modules.Select(@base => @base.GetType().Assembly), null);
            AddLayersCore(dictionary.Where(pair => !PredicateLastLayer(pair, pathInfo)), modelApplication);
            ModelApplicationBase lastLayer = modelApplication.CreatorInstance.CreateModelApplication();
            modelApplication.AddLayer(lastLayer);
        }

        bool PredicateLastLayer(KeyValuePair<string, ResourceInfo> pair, PathInfo pathInfo) {
            var name = pair.Key.EndsWith(ModelStoreBase.ModelDiffDefaultName) ? ModelStoreBase.ModelDiffDefaultName : pair.Key.Substring(pair.Key.LastIndexOf(".") + 1);
            bool nameMatch = (name.EndsWith(Path.GetFileNameWithoutExtension(pathInfo.LocalPath) + ""));
            bool assemblyMatch = Path.GetFileNameWithoutExtension(pathInfo.AssemblyPath) == pair.Value.AssemblyName;
            return nameMatch && assemblyMatch;
        }

        void AddLayersCore(IEnumerable<KeyValuePair<string, ResourceInfo>> layers, ModelApplicationBase modelApplication) {
            IEnumerable<KeyValuePair<string, ResourceInfo>> keyValuePairs = layers;
            foreach (var pair in keyValuePairs) {
                ModelApplicationBase layer = modelApplication.CreatorInstance.CreateModelApplication();
                layer.Id = pair.Key;
                modelApplication.AddLayer(layer);
                var modelXmlReader = new ModelXmlReader();
                foreach (var aspectInfo in pair.Value.AspectInfos) {
                    modelXmlReader.ReadFromString(layer, aspectInfo.AspectName, aspectInfo.Xml);
                }
            }
        }
    }
}