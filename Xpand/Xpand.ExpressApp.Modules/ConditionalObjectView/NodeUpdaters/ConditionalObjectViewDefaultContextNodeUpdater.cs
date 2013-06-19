using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ConditionalObjectView.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalObjectView.NodeUpdaters {
    public class ConditionalObjectViewDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.CustomProcessSelectedItem };
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationConditionalObjectView)node.Application).ConditionalObjectView;
        }
    }
}