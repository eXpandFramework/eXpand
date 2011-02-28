using System;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    public class Mapper {

        public static SimpleTrigger GetSimpleTrigger(IJobTrigger xpandSimpleTrigger, string jobName, Type jobType,
                                                     string jobGroup) {
            var trigger = xpandSimpleTrigger as ISimpleTrigger;
            if (trigger != null) {
                var simpleTrigger = new SimpleTrigger(trigger.Name, jobType.FullName);
                AssignTrigger(simpleTrigger, trigger, jobName, jobType, jobGroup);
                return simpleTrigger;
            }
            return null;
        }

        public static void AssignTrigger(SimpleTrigger jobTrigger, ISimpleTrigger trigger, string jobName, Type type, string jobGroup) {
            jobTrigger.EndTimeUtc = trigger.EndTimeUtc;
            jobTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            if (trigger.RepeatInterval.HasValue)
                jobTrigger.RepeatInterval = trigger.RepeatInterval.Value;
            jobTrigger.Priority = (int)trigger.Priority;
            jobTrigger.CalendarName = trigger.CalendarName;
            jobTrigger.JobDataMap = new JobDataMap();
            jobTrigger.StartTimeUtc = trigger.StartTimeUtc;
            if (trigger.RepeatCount.HasValue)
                jobTrigger.RepeatCount = trigger.RepeatCount.Value;
            jobTrigger.Description = trigger.Description;
            jobTrigger.JobName = jobName;
            jobTrigger.JobGroup = type.FullName;
            jobTrigger.Group = GetGroup(jobName, type,jobGroup);
            trigger.SetFinalFireTimeUtc(jobTrigger.FinalFireTimeUtc);
        }



        public static JobDetail CreateJobDetail(IJobDetail xpandJobDetail) {
            var jobDetail = new JobDetail();
            AssignJobDetail(jobDetail, xpandJobDetail);
            return jobDetail;
        }

        public static void AssignJobDetail(JobDetail jobDetail, IJobDetail xpandJobDetail) {
            jobDetail.Name = xpandJobDetail.Name;
            jobDetail.Description = xpandJobDetail.Description;
            jobDetail.Group = xpandJobDetail.Job.JobType.FullName;
            jobDetail.JobType = xpandJobDetail.Job.JobType;
            jobDetail.RequestsRecovery = xpandJobDetail.RequestsRecovery;
            jobDetail.Volatile = xpandJobDetail.Volatile;
        }


        public static string GetGroup(string jobName, Type jobType, string jobGroup) {
            var format = string.Format("{0}.{1}", jobType.FullName, jobName);
            if (!(string.IsNullOrEmpty(jobGroup)))
                format += "." + jobGroup;
            return format;
        }
    }
}