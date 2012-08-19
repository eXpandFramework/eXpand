using System;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;
namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    public class ScheduledWorkflowStartService : WorkflowStartService<ScheduledWorkflow> {
        protected override bool NeedToStartWorkflow(IObjectSpace objectSpace, ScheduledWorkflow workflow) {
            if (workflow.StartMode == StartMode.OneTime) {
                if (workflow.LaunchHistoryItems.Count == 0 && DateTime.Now.TimeOfDay >= workflow.StartTime) {
                    return true;
                }
            } else if (workflow.StartMode == StartMode.Daily) {
                var historyItem = workflow.LaunchHistoryItems.FirstOrDefault(l => l.LaunchedOn.Date == DateTime.Today);
                if (historyItem == null && DateTime.Now.TimeOfDay >= workflow.StartTime) {
                    var lastStartedWorkflow = workflow.LaunchHistoryItems.OrderByDescending(l => l.LaunchedOn).FirstOrDefault();
                    if (workflow.RecurEveryDays <= 1 || lastStartedWorkflow == null || DateTime.Now >= lastStartedWorkflow.LaunchedOn.Date.AddDays(workflow.RecurEveryDays).Add(workflow.StartTime))
                        return true;
                }
            } else if (workflow.StartMode == StartMode.Weekly) {
                if ((workflow.Monday && DateTime.Now.DayOfWeek == DayOfWeek.Monday) ||
                    (workflow.Tuesday && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) ||
                    (workflow.Wednesday && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) ||
                    (workflow.Thursday && DateTime.Now.DayOfWeek == DayOfWeek.Thursday) ||
                    (workflow.Friday && DateTime.Now.DayOfWeek == DayOfWeek.Friday) ||
                    (workflow.Saturday && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ||
                    (workflow.Sunday && DateTime.Now.DayOfWeek == DayOfWeek.Sunday)) {
                    var historyItem = workflow.LaunchHistoryItems.FirstOrDefault(l => l.LaunchedOn.Date == DateTime.Today);
                    if (historyItem == null && DateTime.Now.TimeOfDay >= workflow.StartTime) {
                        var lastStartedWorkflow = workflow.LaunchHistoryItems.OrderByDescending(l => l.LaunchedOn).FirstOrDefault();
                        var ci = CultureInfo.CurrentCulture;
                        var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                        if (workflow.RecurEveryWeeks <= 1 || lastStartedWorkflow == null || ci.Calendar.GetWeekOfYear(DateTime.Now, weekRule, DayOfWeek.Monday) >= ci.Calendar.GetWeekOfYear(lastStartedWorkflow.LaunchedOn, weekRule, DayOfWeek.Monday) + workflow.RecurEveryWeeks)
                            return true;
                    }
                }
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
