using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Logic.NodeGenerators;

namespace Xpand.ExpressApp.Logic.Model {
    [ModelNodesGenerator(typeof(FrameTemplateContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelFrameTemplateContexts : IModelNode, IModelList<IModelFrameTemplateContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
    }
}