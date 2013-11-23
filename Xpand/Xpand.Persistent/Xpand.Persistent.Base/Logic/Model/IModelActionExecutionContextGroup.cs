using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof(ActionExecutionContextsGroupNodeGenerator))]
    public interface IModelActionExecutionContextGroup : IModelNode, IModelList<IModelActionExecutionContexts> {
    }
}