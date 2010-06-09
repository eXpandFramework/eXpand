using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters {
    public class AdditionalViewControlsDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater {
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationAdditionalViewControls) node.Application).AdditionalViewControls;
        }
    }
}