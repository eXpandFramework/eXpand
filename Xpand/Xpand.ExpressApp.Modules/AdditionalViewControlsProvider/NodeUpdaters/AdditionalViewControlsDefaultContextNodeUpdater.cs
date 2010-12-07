using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.AdditionalViewControlsProvider.NodeUpdaters {
    public class AdditionalViewControlsDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.ViewChanged};
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationAdditionalViewControls) node.Application).AdditionalViewControls;
        }
    }
}