using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    [ModelLogicValidRule(typeof(IActionStateRule))]
    public interface IModelLogicConditionalActionState : IModelLogic {

    }
}