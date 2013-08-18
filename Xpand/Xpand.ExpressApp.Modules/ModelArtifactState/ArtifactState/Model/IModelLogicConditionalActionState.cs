using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Model;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {

    public interface IModelLogicConditionalActionState : IModelLogicContexts {
        IModelActionStateLogicRules Rules { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelActionStateLogicRules : IModelNode, IModelList<IModelActionStateRule> {
    }

}