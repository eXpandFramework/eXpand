using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters {
    public class AdditionalViewControlsDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationAdditionalViewControls) node.Application).AdditionalViewControls;
        }
    }
}