using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.ModelArtifactState.ObjectViews.Model {
    public interface IModelLogicConditionalObjectView : IModelLogicContexts {
        IModelObjectViewLogicRules Rules { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelObjectViewLogicRules : IModelNode, IModelList<IModelObjectViewRule> {
    }

}
