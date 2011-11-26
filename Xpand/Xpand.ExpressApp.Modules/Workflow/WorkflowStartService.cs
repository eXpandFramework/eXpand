using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.Workflow;

namespace Xpand.ExpressApp.Workflow {
    public abstract class WorkflowStartService<T> : BaseTimerService where T : IXpandWorkflowDefinition {
        protected WorkflowStartService(TimeSpan requestsDetectionPeriod)
            : base(requestsDetectionPeriod) {
        }

        protected WorkflowStartService(TimeSpan requestsDetectionPeriod, IObjectSpaceProvider objectSpaceProvider)
            : base(requestsDetectionPeriod, objectSpaceProvider) {
        }

        protected WorkflowStartService()
            : base(TimeSpan.FromMinutes(1)) {
        }

        protected abstract bool NeedToStartWorkflow(IObjectSpace objectSpace, T workflow);

        public override void OnTimer() {
            using (IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
                foreach (T workflow in objectSpace.GetObjects<T>(new BinaryOperator("IsActive", true))) {
                    WorkflowHost host;
                    if (HostManager.Hosts.TryGetValue(workflow.GetUniqueId(), out host)) {
                        if (NeedToStartWorkflow(objectSpace, workflow)) {
                            Guid startWorkflow = host.StartWorkflow(new Dictionary<string, object>());
                            AfterWorkFlowStarted(objectSpace, workflow, startWorkflow);
                            objectSpace.CommitChanges();
                        }
                    }
                }
            }

        }

        protected abstract void AfterWorkFlowStarted(IObjectSpace objectSpace, T workflow, Guid startWorkflow);
    }
}