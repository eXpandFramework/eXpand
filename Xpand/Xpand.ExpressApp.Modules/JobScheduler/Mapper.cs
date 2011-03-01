using System;
using System.Globalization;
using DevExpress.XtraScheduler.Native;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler {
    public class Mapper {
        public Trigger CreateTrigger(IJobTrigger jobTrigger, string jobName, Type jobType,
                                                     string jobGroup) {
            Trigger trigger = CreateTriggerCore(jobTrigger, jobType);
            AssignQuartzTrigger(trigger, jobTrigger, jobName, jobType, jobGroup);
            return trigger;
        }

        Trigger CreateTriggerCore(IJobTrigger jobTrigger, Type jobType) {
            if (jobTrigger is ISimpleTrigger)
                return new SimpleTrigger(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is ICronTrigger)
                return new CronTrigger(jobTrigger.Name, jobType.FullName);
            if (jobTrigger is INthIncludedDayTrigger)
                return new NthIncludedDayTrigger(jobTrigger.Name, jobType.FullName);
            throw new NotImplementedException(jobTrigger.GetType().FullName);
        }


        void AssignQuartzTrigger(SimpleTrigger jobTrigger, ISimpleTrigger trigger) {
            jobTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            if (trigger.RepeatInterval.HasValue)
                jobTrigger.RepeatInterval = trigger.RepeatInterval.Value;
            if (trigger.RepeatCount.HasValue)
                jobTrigger.RepeatCount = trigger.RepeatCount.Value;
            trigger.SetFinalFireTimeUtc(jobTrigger.FinalFireTimeUtc);
        }

        public void AssignQuartzTrigger(Trigger jobTrigger, IJobTrigger trigger, string jobName, Type type, string jobGroup) {
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
                AssignQuartzTrigger((SimpleTrigger)jobTrigger, (ISimpleTrigger)trigger);
            else if (jobTrigger is CronTrigger)
                AssignQuartzTrigger((CronTrigger)jobTrigger, (ICronTrigger)trigger);
            else if (jobTrigger is NthIncludedDayTrigger)
                AssignQuartzTrigger((NthIncludedDayTrigger)jobTrigger, (INthIncludedDayTrigger)trigger);
        }

        string GetCalendarName(IJobTrigger trigger) {
            return trigger.Calendar != null ? trigger.Calendar.Name : null;
        }

        void AssignQuartzTrigger(NthIncludedDayTrigger nthIncludedDayTrigger, INthIncludedDayTrigger trigger) {
            nthIncludedDayTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            nthIncludedDayTrigger.N = trigger.N;
            nthIncludedDayTrigger.IntervalType = (int)trigger.IntervalType;
            nthIncludedDayTrigger.FireAtTime = string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", trigger.FireAtTime.Hours, trigger.FireAtTime.Minutes, trigger.FireAtTime.Seconds);
            nthIncludedDayTrigger.NextFireCutoffInterval = trigger.NextFireCutoffInterval;
            nthIncludedDayTrigger.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RegistryTimeZoneProvider.GetRegistryKeyNameByTimeZoneId(trigger.TimeZone));
            nthIncludedDayTrigger.TriggerCalendarFirstDayOfWeek = trigger.TriggerCalendarFirstDayOfWeek;
            nthIncludedDayTrigger.TriggerCalendarWeekRule = trigger.TriggerCalendarWeekRule;
        }

        void AssignQuartzTrigger(CronTrigger cronTrigger, ICronTrigger trigger) {
            cronTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            cronTrigger.CronExpressionString = trigger.CronExpression;
            cronTrigger.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(trigger.TimeZone.ToString());
        }


        public JobDetail CreateJobDetail(IJobDetail xpandJobDetail) {
            var jobDetail = new JobDetail();
            AssignQuartzJobDetail(jobDetail, xpandJobDetail);
            return jobDetail;
        }

        public void AssignQuartzJobDetail(JobDetail jobDetail, IJobDetail xpandJobDetail) {
            jobDetail.Name = xpandJobDetail.Name;
            jobDetail.Description = xpandJobDetail.Description;
            jobDetail.Group = xpandJobDetail.Job.JobType.FullName;
            jobDetail.JobType = xpandJobDetail.Job.JobType;
            jobDetail.RequestsRecovery = xpandJobDetail.RequestsRecovery;
            jobDetail.Volatile = xpandJobDetail.Volatile;
        }


        public string GetGroup(string jobName, Type jobType, string jobGroup) {
            var format = string.Format("{0}.{1}", jobType.FullName, jobName);
            if (!(string.IsNullOrEmpty(jobGroup)))
                format += "." + jobGroup;
            return format;
        }
    }
}