using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

namespace Xpand.Persistent.Base.Logic.Model{
    public interface IModelObjectChangedExecutionContext : IModelNode {
        string PropertyNames { get; set; }
        string Values { get; set; }
    }
    public class ObjectChangedExecutionContextNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    [ModelNodesGenerator(typeof(ObjectChangedExecutionContextNodeGenerator))]
    public interface IModelObjectChangedExecutionContextGroup : IModelNode, IModelList<IModelObjectChangedExecutionContexts> {
    }

    public class ObjectChangedExecutionContextsNodeGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {
        }
    }

    [ModelNodesGenerator(typeof(ObjectChangedExecutionContextsNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelObjectChangedExecutionContexts : IModelNode, IModelList<IModelObjectChangedExecutionContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}