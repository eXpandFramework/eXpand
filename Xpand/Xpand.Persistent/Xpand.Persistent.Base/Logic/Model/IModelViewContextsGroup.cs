using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof(ViewContextsGroupNodeGenerator))]
    public interface IModelViewContextsGroup : IModelNode, IModelList<IModelViewContexts> {

    }
}