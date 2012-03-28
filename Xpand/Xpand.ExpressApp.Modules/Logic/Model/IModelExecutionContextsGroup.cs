using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(ExecutionContextsGroupNodeGenerator))]
    public interface IModelExecutionContextsGroup : IModelNode, IModelList<IModelExecutionContexts> {
    }

    [ModelNodesGenerator(typeof(ActionExecutionContextsGroupNodeGenerator))]
    public interface IModelActionExecutionContextGroup : IModelNode, IModelList<IModelActionExecutionContexts> {
    }
    


}