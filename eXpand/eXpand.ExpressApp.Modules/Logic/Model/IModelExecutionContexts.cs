using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof (ExecutionContextNodeGenerator))]
// ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelExecutionContexts : IModelNode, IModelList<IModelExecutionContext>, IRule {
// ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}