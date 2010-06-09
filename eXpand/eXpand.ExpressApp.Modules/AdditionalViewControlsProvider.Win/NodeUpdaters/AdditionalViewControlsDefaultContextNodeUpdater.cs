using System.Collections.Generic;
using DevExpress.ExpressApp.Model.Core;
using eXpand.ExpressApp.AdditionalViewControlsProvider.Model;
using eXpand.ExpressApp.Logic;
using eXpand.ExpressApp.Logic.Model;
using eXpand.ExpressApp.Logic.NodeUpdaters;

namespace eXpand.ExpressApp.AdditionalViewControlsProvider.Win.NodeUpdaters {
    public class AdditionalViewControlsDefaultContextNodeUpdater : LogicDefaultContextNodeUpdater {
        protected override List<ExecutionContext> GetExecutionContexts() {
            return new List<ExecutionContext> { ExecutionContext.CurrentObjectChanged, ExecutionContext.ObjectChanged, ExecutionContext.TemplateViewChanged };
        }

        protected override IModelLogic GetModelLogicNode(ModelNode node) {
            return ((IModelApplicationAdditionalViewControls) node.Application).AdditionalViewControls;
        }
    }
}