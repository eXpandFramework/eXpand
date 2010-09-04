using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ConditionalDetailViews.Model;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
    public class ConditionalDetailViewDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater
    {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationConditionalDetailView)node.Application).ConditionalDetailView;
        }
    }
}