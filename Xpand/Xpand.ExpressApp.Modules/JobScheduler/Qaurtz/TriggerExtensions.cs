using System;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using Quartz;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public static class TriggerExtensions {
        public static void AssignQuartzTrigger(this SimpleTrigger jobTrigger, ISimpleTrigger trigger) {
            jobTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            if (trigger.RepeatInterval.HasValue)
                jobTrigger.RepeatInterval = trigger.RepeatInterval.Value;
            if (trigger.RepeatCount.HasValue)
                jobTrigger.RepeatCount = trigger.RepeatCount.Value;
            trigger.SetFinalFireTimeUtc(jobTrigger.FinalFireTimeUtc);
        }

        static string GetCalendarName(IJobTrigger trigger) {
            return trigger.Calendar != null ? trigger.Calendar.Name : null;
        }
        public static string GetGroup(string jobName, Type jobType, string jobGroup) {
            var format = string.Format("{0}.{1}", jobType.FullName, jobName);
            if (!(string.IsNullOrEmpty(jobGroup)))
                format += "." + jobGroup;
            return format;
        }

        public static Trigger CreateTrigger(this IJobTrigger jobTrigger, string jobName, Type jobType,
                                                     string jobGroup) {
            Trigger trigger = CreateTriggerCore(jobTrigger, jobType);
            trigger.AssignQuartzTrigger(jobTrigger, jobName, jobType, jobGroup);
            return trigger;
        }

        static Trigger CreateTriggerCore(this IJobTrigger jobTrigger, Type jobType) {
            Trigger trigger = null;
            if (jobTrigger is ISimpleTrigger)
                trigger = new SimpleTrigger(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is ICronTrigger)
                trigger = new CronTrigger(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is INthIncludedDayTrigger)
                trigger = new NthIncludedDayTrigger(jobTrigger.Name, jobType.FullName);

            if (trigger != null) {
                trigger.AddTriggerListener(new XpandTriggerListener().Name);
                return trigger;
            }
            throw new NotImplementedException(jobTrigger.GetType().FullName);
        }
        public static void AssignQuartzTrigger(this Trigger jobTrigger, IJobTrigger trigger, string jobName, Type type, string jobGroup) {
            jobTrigger.EndTimeUtc = trigger.EndTimeUtc;
            jobTrigger.Priority = (int)trigger.Priority;
            jobTrigger.CalendarName = GetCalendarName(trigger);
            jobTrigger.JobDataMap = new JobDataMap();
            jobTrigger.StartTimeUtc = trigger.StartTimeUtc;
            jobTrigger.Description = trigger.Description;
            jobTrigger.JobName = jobName;
            jobTrigger.JobGroup = type.FullName;
            jobTrigger.Group = GetGroup(jobName, type, jobGroup);
            if (jobTrigger is SimpleTrigger)
                ((SimpleTrigger)jobTrigger).AssignQuartzTrigger((ISimpleTrigger)trigger);
            else if (jobTrigger is CronTrigger)
                ((CronTrigger)jobTrigger).AssignQuartzTrigger((ICronTrigger)trigger);
            else if (jobTrigger is NthIncludedDayTrigger)
                ((NthIncludedDayTrigger)jobTrigger).AssignQuartzTrigger((INthIncludedDayTrigger)trigger);
        }

        public static void AssignQuartzTrigger(this NthIncludedDayTrigger nthIncludedDayTrigger, INthIncludedDayTrigger trigger) {
            nthIncludedDayTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            nthIncludedDayTrigger.N = trigger.N;
            nthIncludedDayTrigger.IntervalType = (int)trigger.IntervalType;
            nthIncludedDayTrigger.FireAtTime = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", trigger.FireAtTime.Hours, trigger.FireAtTime.Minutes, trigger.FireAtTime.Seconds);
            nthIncludedDayTrigger.NextFireCutoffInterval = trigger.NextFireCutoffInterval;
            nthIncludedDayTrigger.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(trigger.TimeZone));
            nthIncludedDayTrigger.TriggerCalendarFirstDayOfWeek = trigger.TriggerCalendarFirstDayOfWeek;
            nthIncludedDayTrigger.TriggerCalendarWeekRule = trigger.TriggerCalendarWeekRule;
        }

        public static void AssignQuartzTrigger(this CronTrigger cronTrigger, ICronTrigger trigger) {
            cronTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            cronTrigger.CronExpressionString = trigger.CronExpression;
            cronTrigger.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone.ToString());
        }



    }
}
