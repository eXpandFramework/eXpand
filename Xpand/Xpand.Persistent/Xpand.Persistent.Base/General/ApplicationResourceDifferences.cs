using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using Fasterflect;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.Persistent.Base.General {
    public interface IModelApplicationResourceDifferences {
        IModelApplication ResourceDifferencesApplication { get; set; }
    }
    public class ApplicationDifferencesUpdater : ModelNodesGeneratorUpdater<ModelBOModelClassNodesGenerator> {
        public override void UpdateNode(ModelNode node) {
            ((IModelApplicationResourceDifferences)node.Application).ResourceDifferencesApplication = ModelApplication(node);
        }

        IModelApplication ModelApplication(IModelNode modelNode) {
            var node = ((ModelNode)modelNode);
            var modelApplication = node.CreatorInstance.CreateModelApplication();
            var modelStores = Modules(node).Select(module => new ResourcesModelStore(module.GetType().Assembly)).Cast<ModelStoreBase>().ToList();
            if (InterfaceBuilder.RuntimeMode && !XpandModuleBase.IsHosted) {
                var path = ApplicationHelper.Instance.Application.GetPropertyValue("UserModelDifferenceFilePath") + "";
                var file = Path.Combine(path, "model.user.xafml");
                if (File.Exists(file))
                    modelStores.Add(new FileModelStore(file, "Model.User"));
            }
            foreach (var resourcesModelStore in modelStores) {
                resourcesModelStore.Load(modelApplication);
            }
            return modelApplication as IModelApplication;
        }

        IEnumerable<ModuleBase> Modules(ModelNode node) {
            var moduleBases = ((IModelSources)node.Application).Modules;
            return moduleBases.Any() ? moduleBases : ApplicationHelper.Instance.Application.Modules;
        }

    }

}
