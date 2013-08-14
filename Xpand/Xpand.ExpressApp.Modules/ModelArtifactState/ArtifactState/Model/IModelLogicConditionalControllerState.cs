using Xpand.ExpressApp.ModelArtifactState.ControllerState;
using Xpand.ExpressApp.ModelArtifactState.ControllerState.Logic;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Logic;
using Xpand.Persistent.Base.Logic.Model;

namespace Xpand.ExpressApp.ModelArtifactState.ArtifactState.Model {
    [ModelLogicValidRule(typeof (IControllerStateRule))]
    [LogicInstaller(typeof (ControllerStateLogicInstaller))]
    public interface IModelLogicConditionalControllerState : IModelLogic {
    }
}