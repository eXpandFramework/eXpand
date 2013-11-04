using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;

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
