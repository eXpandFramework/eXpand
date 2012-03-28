using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(ExecutionContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelExecutionContexts : IModelNode, IModelList<IModelExecutionContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }
    [ModelNodesGenerator(typeof(ActionExecutionContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelActionExecutionContexts : IModelNode, IModelList<IModelActionExecutionContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }

}