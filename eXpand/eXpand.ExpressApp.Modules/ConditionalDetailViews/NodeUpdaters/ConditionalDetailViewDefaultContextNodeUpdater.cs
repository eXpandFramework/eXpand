using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.ConditionalDetailViews.Model;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.ConditionalDetailViews.NodeUpdaters {
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