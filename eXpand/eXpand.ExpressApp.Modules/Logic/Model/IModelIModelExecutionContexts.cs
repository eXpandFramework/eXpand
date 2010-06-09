using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.Logic.NodeGenerators;

namespace eXpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof (ExecutionContextNodeGenerator))]
    public interface IModelIModelExecutionContexts : IModelNode, IModelList<IModelExecutionContext>, IRule {
    }
}