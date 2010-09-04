using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ArtifactState.Model;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateDefaultGroupContextNodeUpdater:LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalActionState;
        }
    }
}