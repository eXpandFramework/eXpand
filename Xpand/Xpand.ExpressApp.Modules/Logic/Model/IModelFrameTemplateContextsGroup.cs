using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(FrameTemplateContextsGroupNodeGenerator))]
    public interface IModelFrameTemplateContextsGroup : IModelNode, IModelList<IModelFrameTemplateContexts> {
    }
}