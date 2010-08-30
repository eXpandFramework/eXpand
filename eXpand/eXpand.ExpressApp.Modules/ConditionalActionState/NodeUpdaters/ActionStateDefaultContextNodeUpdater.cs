using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.ArtifactState.NodeUpdaters;
using eXpand.ExpressApp.Logic.Model;

namespace eXpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateDefaultContextNodeUpdater : ArtifactStateDefaultContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalActionState;
        }
    }
}