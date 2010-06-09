using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ArtifactState.NodeUpdaters;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ConditionalControllerState.NodeUpdaters {
    public class ControllerStateDefaultContextNodeUpdater : ArtifactStateDefaultContextNodeUpdater{
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalControllerState;
        }
    }
}