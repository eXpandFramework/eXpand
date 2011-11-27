using System;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Spi;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public static class TriggerExtensions {
        public static void AssignQuartzTrigger(this SimpleTriggerImpl jobTrigger, IXpandSimpleTrigger trigger) {
            jobTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            if (trigger.RepeatInterval.HasValue)
                jobTrigger.RepeatInterval = trigger.RepeatInterval.Value;
            if (trigger.RepeatCount.HasValue)
                jobTrigger.RepeatCount = trigger.RepeatCount.Value;
            trigger.SetFinalFireTimeUtc(jobTrigger.FinalFireTimeUtc);
        }

        static string GetCalendarName(IXpandJobTrigger trigger) {
            return trigger.Calendar != null ? trigger.Calendar.Name : null;
        }
        public static string GetGroup(string jobName, Type jobType, string jobGroup) {
            var format = string.Format("{0}.{1}", jobType.FullName, jobName);
            if (!(string.IsNullOrEmpty(jobGroup)))
                format += "." + jobGroup;
            return format;
        }

        public static IOperableTrigger CreateTrigger(this IXpandJobTrigger jobTrigger, string jobName, Type jobType,
                                                     string jobGroup) {
            IOperableTrigger trigger = CreateTriggerCore(jobTrigger, jobType);
            trigger.AssignQuartzTrigger(jobTrigger, jobName, jobType, jobGroup);
            return trigger;
        }

        static IOperableTrigger CreateTriggerCore(this IXpandJobTrigger jobTrigger, Type jobType) {
            IOperableTrigger trigger = null;
            if (jobTrigger is IXpandSimpleTrigger)
                trigger = new SimpleTriggerImpl(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is IXpandCronTrigger)
                trigger = new CronTriggerImpl(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is INthIncludedDayTrigger)
                trigger = new NthIncludedDayTrigger(jobTrigger.Name, jobType.FullName);

            if (trigger != null) {
                return trigger;
            }
            throw new NotImplementedException(jobTrigger.GetType().FullName);
        }
        public static void AssignQuartzTrigger(this IOperableTrigger jobTrigger, IXpandJobTrigger trigger, string jobName, Type type, string jobGroup) {
            jobTrigger.EndTimeUtc = trigger.EndTimeUtc;
            jobTrigger.Priority = (int)trigger.Priority;
            jobTrigger.CalendarName = GetCalendarName(trigger);
            jobTrigger.JobDataMap = new JobDataMap();
            jobTrigger.StartTimeUtc = trigger.StartTimeUtc;
            jobTrigger.Description = trigger.Description;
            jobTrigger.JobKey = new JobKey(jobName, type.FullName);
            jobTrigger.Key = new TriggerKey(jobTrigger.Key.Name, GetGroup(jobName, type, jobGroup));
            if (jobTrigger is SimpleTriggerImpl)
                ((SimpleTriggerImpl)jobTrigger).AssignQuartzTrigger((IXpandSimpleTrigger)trigger);
            else if (jobTrigger is CronTriggerImpl)
                ((CronTriggerImpl)jobTrigger).AssignQuartzTrigger((IXpandCronTrigger)trigger);
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

        public static void AssignQuartzTrigger(this CronTriggerImpl cronTrigger, IXpandCronTrigger trigger) {
            cronTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            cronTrigger.CronExpressionString = trigger.CronExpression;
            // http://devexpress.com/Support/Center/p/S133718.aspx?searchtext=timezoneid+number
            // Fetches the Windows name of the specified Scheduler time zone from the registry.
            String timeZoneKey = RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(trigger.TimeZone);
            cronTrigger.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneKey);
        }



    }
}
