using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalControllerState.NodeUpdaters {
    public class ControllerStateDefaultGroupContextNodeUpdater:LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalControllerState;
        }
    }
}