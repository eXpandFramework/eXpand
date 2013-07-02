using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof(FrameTemplateContextsGroupNodeGenerator))]
    public interface IModelFrameTemplateContextsGroup : IModelNode, IModelList<IModelFrameTemplateContexts> {
    }
}