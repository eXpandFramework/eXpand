using System;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    internal class Mapper {

        public static SimpleTrigger GetSimpleTrigger(IJobTrigger xpandSimpleTrigger, string jobName, string jobgroup) {
            var trigger = xpandSimpleTrigger as IXpandSimpleTrigger;
            if (trigger != null) {
                var simpleTrigger = new SimpleTrigger(trigger.Name, trigger.Group);
                AssignTrigger(simpleTrigger, trigger, jobName,jobgroup);
                return simpleTrigger;
            }
            return null;
        }

        public static void AssignTrigger(SimpleTrigger jobTrigger,IXpandSimpleTrigger trigger, string jobName, string jobgroup) {
            jobTrigger.EndTimeUtc = trigger.EndTimeUtc;
            jobTrigger.MisfireInstruction = (int)trigger.MisfireInstruction;
            jobTrigger.Volatile = trigger.Volatile;
            jobTrigger.RepeatInterval = trigger.RepeatInterval;
            jobTrigger.Priority = (int)trigger.Priority;
            jobTrigger.CalendarName = trigger.CalendarName;
            jobTrigger.JobDataMap = new JobDataMap();
            jobTrigger.StartTimeUtc = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow);
            jobTrigger.RepeatCount = trigger.RepeatCount;
            jobTrigger.Description = trigger.Description;
            jobTrigger.JobName = jobName;
            jobTrigger.JobGroup = jobgroup;
        }


        public static JobDetail GetJobDetail(JobDetail jobDetail) {
            return new JobDetail {
                Name = jobDetail.Name,
                Durable = jobDetail.Durable,
                Description = jobDetail.Description,
                Group = jobDetail.Group,
                JobType = jobDetail.JobType,
                RequestsRecovery = jobDetail.RequestsRecovery,
                Volatile = jobDetail.Volatile
            };

        }

        public static JobDetail GetJobDetail(IJobDetail xpandJobDetail) {
            return new JobDetail {
                Name = xpandJobDetail.Name,
                Durable = xpandJobDetail.Durable,
                Description = xpandJobDetail.Description,
                Group = xpandJobDetail.Group,
                JobType = xpandJobDetail.JobType,
                RequestsRecovery = xpandJobDetail.RequestsRecovery,
                Volatile = xpandJobDetail.Volatile
            };

        }
    }
}