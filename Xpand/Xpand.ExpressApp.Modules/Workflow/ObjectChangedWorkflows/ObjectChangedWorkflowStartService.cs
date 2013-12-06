using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class ObjectChangedWorkflowStartService : WorkflowStartService<ObjectChangedWorkflow> {
        public ObjectChangedWorkflowStartService()
            : base(TimeSpan.FromMinutes(1)) {
        }
        public ObjectChangedWorkflowStartService(TimeSpan requestsDetectionPeriod) : base(requestsDetectionPeriod) { }
        protected override bool NeedToStartWorkflow(IObjectSpace objectSpace, ObjectChangedWorkflow workflow) {
            return workflow.ExecutionDomain==ExecutionDomain.Server;
        }

        protected override void AfterWorkFlowStarted(IObjectSpace objectSpace, ObjectChangedWorkflow workflow, Guid startWorkflow) {

        }
    }
}
