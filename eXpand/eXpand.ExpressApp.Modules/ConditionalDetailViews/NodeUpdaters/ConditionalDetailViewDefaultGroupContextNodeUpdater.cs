using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ConditionalDetailViews.Model;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
    public class ConditionalDetailViewDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater
    {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationConditionalDetailView)node.Application).ConditionalDetailView;
        }
    }
}