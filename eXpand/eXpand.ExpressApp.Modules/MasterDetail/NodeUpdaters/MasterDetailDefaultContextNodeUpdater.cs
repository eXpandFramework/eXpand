using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;
using eXpand.ExpressApp.MasterDetail.Model;

namespace eXpand.ExpressApp.MasterDetail.NodeUpdaters {
    public class MasterDetailDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.ObjectChanged,ExecutionContext.CurrentObjectChanged,ExecutionContext.ControllerActivated};
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationMasterDetail)node.Application).MasterDetail;
        }
    }
}