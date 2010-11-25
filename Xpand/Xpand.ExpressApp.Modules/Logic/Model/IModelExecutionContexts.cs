using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(ExecutionContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelExecutionContexts : IModelNode, IModelList<IModelExecutionContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}