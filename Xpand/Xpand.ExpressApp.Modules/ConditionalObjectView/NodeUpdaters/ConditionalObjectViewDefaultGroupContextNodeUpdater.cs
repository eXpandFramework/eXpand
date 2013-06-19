using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalObjectView.NodeUpdaters {
    public class ConditionalObjectViewDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationConditionalObjectView)node.Application).ConditionalObjectView;
        }
    }
}