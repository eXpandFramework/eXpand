using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ArtifactState.Model;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalActionState.NodeUpdaters {
    public class ActionStateDefaultGroupContextNodeUpdater:LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationModelArtifactState)node.Application).ModelArtifactState.ConditionalActionState;
        }
    }
}