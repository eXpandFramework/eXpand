using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.CommonServices;

namespace Xpand.ExpressApp.Workflow {
    public class ScheduledWorkflowDefinitionProvider : WorkflowDefinitionProvider {
        public ScheduledWorkflowDefinitionProvider(Type workflowDefinitionType) : base(workflowDefinitionType) { }
        public ScheduledWorkflowDefinitionProvider(Type workflowDefinitionType, IObjectSpaceProvider objectSpaceProvider) : base(workflowDefinitionType, objectSpaceProvider) { }
        public override IList<IWorkflowDefinition> GetDefinitions() {
            IList<IWorkflowDefinition> result = base.GetDefinitions();
            IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace();
            foreach (ScheduledWorkflow workflow in objectSpace.GetObjects<ScheduledWorkflow>()) {
                result.Add(workflow);
            }
            return result;
        }
    }
}
