using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(ViewContextsGroupNodeGenerator))]
    public interface IModelViewContextsGroup : IModelNode, IModelList<IModelViewContexts> {

    }
}