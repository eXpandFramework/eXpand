using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.ConditionalDetailViews.Model;
using Xpand.ExpressApp.Logic;
using Xpand.ExpressApp.Logic.Model;
using Xpand.ExpressApp.Logic.NodeUpdaters;

namespace Xpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
    public class ConditionalDetailViewDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater
    {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.CustomProcessSelectedItem};
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationConditionalDetailView)node.Application).ConditionalDetailView;
        }
    }
}