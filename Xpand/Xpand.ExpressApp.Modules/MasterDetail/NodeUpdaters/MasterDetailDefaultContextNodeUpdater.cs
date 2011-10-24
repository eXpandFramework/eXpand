using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;
using Xpand.ExpressApp.MasterDetail.Model;

namespace Xpand.ExpressApp.MasterDetail.NodeUpdaters {
    public class MasterDetailDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.ObjectSpaceObjectChanged,ExecutionContext.CurrentObjectChanged,ExecutionContext.ControllerActivated};
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationMasterDetail)node.Application).MasterDetail;
        }
    }
}