using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.ArtifactState.NodeUpdaters;
using Xpand.ExpressApp.Logic.Model;

namespace Xpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateDefaultContextNodeUpdater : ArtifactStateDefaultContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalActionState;
        }
    }
}