using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Model;
using Xpand.Persistent.Base.Logic.Model;
using Xpand.Persistent.Base.Logic.NodeGenerators;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    public interface IModelLogicConditionalControllerState : IModelLogicContexts {
        IModelControllerStateLogicRules Rules { get; }
    }
    [ModelNodesGenerator(typeof(LogicRulesNodesGenerator))]
    public interface IModelControllerStateLogicRules : IModelNode, IModelList<IModelControllerStateRule> {
    }

}