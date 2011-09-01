using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Workflow;

namespace Xpand.ExpressApp.Workflow {
    public class ScheduledWorkflowStartService : BaseTimerService {
        private bool NeedToStartWorkflow(IObjectSpace objectSpace, ScheduledWorkflow workflow) {
            if (workflow.StartMode == StartMode.OneTime) {
                if (workflow.LaunchHistoryItems.Count == 0) {
                    return true;
                }
            } else if (workflow.StartMode == StartMode.Daily) {
                var historyItem = objectSpace.FindObject<ScheduledWorkflowLaunchHistory>(CriteriaOperator.Parse("GetDate(LaunchedOn) = ?", DateTime.Today));
                if (historyItem == null && DateTime.Now.TimeOfDay > workflow.StartTime) {
                    return true;
                }
            } else if (workflow.StartMode == StartMode.Weekly) {
                throw new NotImplementedException();
            }
            return false;
        }
        public ScheduledWorkflowStartService()
            : base(TimeSpan.FromMinutes(1)) {
        }
        public ScheduledWorkflowStartService(TimeSpan requestsDetectionPeriod) : base(requestsDetectionPeriod) { }
        public override void OnTimer() {
            using (IObjectSpace objectSpace = ObjectSpaceProvider.CreateObjectSpace()) {
                foreach (ScheduledWorkflow workflow in objectSpace.GetObjects<ScheduledWorkflow>(new BinaryOperator("IsActive", true))) {
                    WorkflowHost host;
                    if (HostManager.Hosts.TryGetValue(workflow.GetUniqueId(), out host)) {
                        if (NeedToStartWorkflow(objectSpace, workflow)) {
                            host.StartWorkflow(new Dictionary<string, object>());
                            var historyItem = objectSpace.CreateObject<ScheduledWorkflowLaunchHistory>();
                            historyItem.Workflow = workflow;
                            historyItem.LaunchedOn = DateTime.Now;
                            objectSpace.CommitChanges();
                        }
                    }
                }
            }
        }
    }
}
