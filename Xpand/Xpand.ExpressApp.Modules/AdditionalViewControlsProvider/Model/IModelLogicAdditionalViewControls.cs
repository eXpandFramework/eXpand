using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.Model {
    public interface IModelLogicAdditionalViewControls:IModelNode {
        IModelAdditionalViewControlsLogicRules Rules { get; }
        IModelExecutionContextsGroup ExecutionContextsGroup { get; }
        IModelViewContextsGroup ViewContextsGroup { get; }
        IModelFrameTemplateContextsGroup FrameTemplateContextsGroup { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelAdditionalViewControlsLogicRules : IModelNode, IModelList<IModelAdditionalViewControlsRule> {
    }

}