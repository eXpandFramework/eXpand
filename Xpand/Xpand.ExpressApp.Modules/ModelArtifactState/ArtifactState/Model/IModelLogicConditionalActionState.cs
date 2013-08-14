using Xpand.ExpressApp.ModelArtifactState.ActionState;
using Xpand.ExpressApp.ModelArtifactState.ActionState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    [ModelLogicValidRule(typeof (IActionStateRule))]
    [LogicInstaller(typeof (ActionStateLogicInstaller))]
    public interface IModelLogicConditionalActionState : IModelLogic {
    }
}