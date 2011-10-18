using System;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Workflow.ObjectChangedWorkflows {
    public class ObjectChangedWorkflowStartService : WorkflowStartService<ObjectChangedWorkflow> {
        public ObjectChangedWorkflowStartService()
            : base(TimeSpan.FromMinutes(1)) {
        }
        public ObjectChangedWorkflowStartService(TimeSpan requestsDetectionPeriod) : base(requestsDetectionPeriod) { }
        protected override bool NeedToStartWorkflow(IObjectSpace objectSpace, ObjectChangedWorkflow workflow) {
            return true;
        }

        protected override void AfterWorkFlowStarted(IObjectSpace objectSpace, ObjectChangedWorkflow workflow, Guid startWorkflow) {

        }
    }
}
