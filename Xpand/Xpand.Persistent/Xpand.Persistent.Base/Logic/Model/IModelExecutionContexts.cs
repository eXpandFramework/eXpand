using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
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