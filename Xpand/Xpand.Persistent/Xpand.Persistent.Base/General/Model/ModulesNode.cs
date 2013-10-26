using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.General.Model {

    [ModelNodesGenerator(typeof (ModuleNodesGenerator))]
    public interface IModelModules : IModelNode, IModelList<IModelModule> {
    }

    public interface IModelApplicationModule {
        [ModelPersistentName("Modules")]
        IModelModules ModulesList { get;  }
    }
    [KeyProperty("Name"), DisplayProperty("Name")]
    public interface IModelModule:IModelNode {
        string Name { get; set; }
    }

    public class ModuleNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
            foreach (var module in ((IModelSources) node.Application).Modules) {
                var modelModule = node.AddNode<IModelModule>();
                modelModule.Name = module.GetType().FullName;
            }
        }
    }
}