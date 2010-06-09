using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalControllerState.NodeUpdaters {
    public class ControllerStateDefaultGroupContextNodeUpdater:LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalControllerState;
        }
    }
}