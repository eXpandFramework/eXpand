using System;
using System.Globalization;
using System.Linq;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    public class ScheduledWorkflowStartService : WorkflowStartService<ScheduledWorkflow> {
        protected override bool NeedToStartWorkflow(IObjectSpace objectSpace, ScheduledWorkflow workflow) {
            foreach (var schedule in workflow.LaunchScheduleItems) {
                switch (schedule.StartMode) {
                    case StartMode.OneTime:
                        if (NeedToStartOneTime(workflow, schedule)) return true;
                        break;
                    case StartMode.Daily: {
                            if (NeedToStartDaily(workflow, schedule)) return true;
                        }
                        break;
                    case StartMode.Weekly:
                        if (WeeklyDayMatch(schedule)) {
                            if (workflow.LaunchScheduleItems.Count == 0)
                                return false;
                            if (NeedToStartWeekly(workflow, schedule)) return true;
                        }
                        break;
                }
            }
            return false;
        }

        bool WeeklyDayMatch(ScheduledWorkflowLaunchSchedule schedule) {
            return (schedule.Monday && DateTime.Now.DayOfWeek == DayOfWeek.Monday) ||
                   (schedule.Tuesday && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) ||
                   (schedule.Wednesday && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday) ||
                   (schedule.Thursday && DateTime.Now.DayOfWeek == DayOfWeek.Thursday) ||
                   (schedule.Friday && DateTime.Now.DayOfWeek == DayOfWeek.Friday) ||
                   (schedule.Saturday && DateTime.Now.DayOfWeek == DayOfWeek.Saturday) ||
                   (schedule.Sunday && DateTime.Now.DayOfWeek == DayOfWeek.Sunday);
        }

        bool NeedToStartWeekly(ScheduledWorkflow workflow, ScheduledWorkflowLaunchSchedule schedule) {
            var lastLaunch = LastLaunch(workflow);
            var currentTime = DateTime.Now.TimeOfDay;

            var nextLaunch = NextLaunch(workflow, lastLaunch, currentTime);
            if (currentTime >= schedule.StartTime && nextLaunch == schedule.StartTime && lastLaunch <= schedule.StartTime) {
                var lastStartedWorkflow = workflow.LaunchHistoryItems.OrderByDescending(l => l.LaunchedOn).FirstOrDefault();
                var ci = CultureInfo.CurrentCulture;
                var weekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
                if (WeekRule(schedule, lastStartedWorkflow, ci, weekRule))
                    return true;
            }
            return false;
        }

        bool WeekRule(ScheduledWorkflowLaunchSchedule schedule, ScheduledWorkflowLaunchHistory lastStartedWorkflow, CultureInfo ci, CalendarWeekRule weekRule) {
            return schedule.RecurEveryWeeks <= 1 || lastStartedWorkflow == null ||
                   ci.Calendar.GetWeekOfYear(DateTime.Now, weekRule, DayOfWeek.Monday) >=
                   ci.Calendar.GetWeekOfYear(lastStartedWorkflow.LaunchedOn, weekRule, DayOfWeek.Monday) +
                   schedule.RecurEveryWeeks;
        }

        TimeSpan LastLaunch(ScheduledWorkflow workflow) {
            var lastLaunch = workflow.LaunchHistoryItems.OrderByDescending(l => l.LaunchedOn)
                                     .Where(l => l.LaunchedOn.Date == DateTime.Today)
                                     .Select(l => l.LaunchedOn.TimeOfDay)
                                     .FirstOrDefault();
            return lastLaunch;
        }

        bool NeedToStartDaily(ScheduledWorkflow workflow, ScheduledWorkflowLaunchSchedule schedule) {
            var historyItem = workflow.LaunchHistoryItems.FirstOrDefault(l => l.LaunchedOn.Date == DateTime.Today);
            if (historyItem == null && DateTime.Now.TimeOfDay >= schedule.StartTime) {
                var lastStartedWorkflow = workflow.LaunchHistoryItems.OrderByDescending(l => l.LaunchedOn).FirstOrDefault();
                if (schedule.RecurEveryDays <= 1 || lastStartedWorkflow == null ||
                    DateTime.Now >= lastStartedWorkflow.LaunchedOn.Date.AddDays(schedule.RecurEveryDays).Add(schedule.StartTime))
                    return true;
            }
            return false;
        }

        bool NeedToStartOneTime(ScheduledWorkflow workflow, ScheduledWorkflowLaunchSchedule schedule) {
            return workflow.LaunchHistoryItems.Count == 0 && DateTime.Now.TimeOfDay >= schedule.StartTime;
        }

        TimeSpan NextLaunch(ScheduledWorkflow workflow, TimeSpan lastLaunch, TimeSpan currentTime) {
            return lastLaunch != TimeSpan.Zero ? workflow.LaunchScheduleItems.Aggregate((x, y) => Math.Abs(x.StartTime.Ticks - lastLaunch.Ticks) < Math.Abs(y.StartTime.Ticks - lastLaunch.Ticks) ? x : y).StartTime
                                      : workflow.LaunchScheduleItems.Aggregate((x, y) => Math.Abs(x.StartTime.Ticks - currentTime.Ticks) > Math.Abs(y.StartTime.Ticks - currentTime.Ticks) ? x : y).StartTime;
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
