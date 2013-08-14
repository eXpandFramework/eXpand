using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.Persistent.Base.Logic.Model {
    [ModelNodesGenerator(typeof(FrameTemplateContextNodeGenerator))]
    // ReSharper disable PossibleInterfaceMemberAmbiguity
    public interface IModelFrameTemplateContexts : IModelNode, IModelList<IModelFrameTemplateContext>, IRule {
        // ReSharper restore PossibleInterfaceMemberAmbiguity
        [Browsable(false)]
        FrameTemplateContext FrameTemplateContext { get; set; }
    }
}