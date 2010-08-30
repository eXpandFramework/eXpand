using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using eXpand.ExpressApp.MasterDetail.Model;

namespace eXpand.ExpressApp.MasterDetail.NodeUpdaters {
    public class MasterDetailDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater{
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationMasterDetail)node.Application).MasterDetail;
        }
    }
}