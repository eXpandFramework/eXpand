using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Model;

namespace Xpand.ExpressApp.MasterDetail.NodeUpdaters {
    public class MasterDetailDefaultGroupContextNodeUpdater : LogicDefaultGroupContextNodeUpdater{
        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationMasterDetail)node.Application).MasterDetail;
        }
    }
}