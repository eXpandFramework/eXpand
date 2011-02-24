using System;
using Quartz;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler {
    internal class Mapper {
        public static SimpleTrigger GetSimpleTrigger(IJobTrigger xpandSimpleTrigger, string jobName, string jobgroup) {
            var trigger = xpandSimpleTrigger as IXpandSimpleTrigger;
            if (trigger != null) {
                var simpleTrigger = new SimpleTrigger(trigger.Name, trigger.Group) {
                    EndTimeUtc = trigger.EndTimeUtc,
                    MisfireInstruction = (int)trigger.MisfireInstruction,
                    Volatile = trigger.Volatile,
                    RepeatInterval = trigger.RepeatInterval,
                    Priority = (int)trigger.Priority,
                    CalendarName = trigger.CalendarName,
                    JobDataMap = new JobDataMap(),
                    StartTimeUtc = TriggerUtils.GetEvenMinuteDate(DateTime.UtcNow),
                    RepeatCount = trigger.RepeatCount,
                    Description = trigger.Description,
                    JobName = jobName,
                    JobGroup = jobgroup
                };
                return simpleTrigger;
            }
            return null;
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