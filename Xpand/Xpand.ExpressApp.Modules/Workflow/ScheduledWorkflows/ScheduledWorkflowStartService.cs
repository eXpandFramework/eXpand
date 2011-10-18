using System;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;

namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    public class ScheduledWorkflowStartService : WorkflowStartService<ScheduledWorkflow> {
        protected override bool NeedToStartWorkflow(IObjectSpace objectSpace, ScheduledWorkflow workflow) {
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

        protected override void AfterWorkFlowStarted(IObjectSpace objectSpace, ScheduledWorkflow workflow, Guid startWorkflow) {
            var historyItem = objectSpace.CreateObject<ScheduledWorkflowLaunchHistory>();
            historyItem.Workflow = workflow;
            historyItem.LaunchedOn = DateTime.Now;
        }
    }
}
