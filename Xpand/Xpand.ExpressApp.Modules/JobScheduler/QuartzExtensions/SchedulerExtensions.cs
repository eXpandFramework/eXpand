using System;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public static class SchedulerExtensions {
        public const string TriggerJobListenersOn = "TriggerJobListenersOn";
        public const string TriggerTriggerJobListenersOn = "TriggerTriggerJobListenersOn";

        public static IJobDetail GetJobDetail(this IScheduler scheduler, IXpandJobDetail jobDetail) {
            return scheduler.GetJobDetail(jobDetail.Name, jobDetail.Job.JobType);
        }

        public static void TriggerJob(this IScheduler scheduler, IXpandJobDetail jobDetail) {
            scheduler.TriggerJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public static IList<ITrigger> GetTriggersOfJob(this IScheduler scheduler, IXpandJobDetail jobDetail) {
            return scheduler.GetTriggersOfJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public static bool DeleteJob(this IScheduler scheduler, IXpandJobDetail jobDetail) {
            return scheduler.DeleteJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public static bool UnscheduleJob(this IScheduler scheduler, string triggerName, Type jobType, string jobName, string jobGroup) {
            return scheduler.UnscheduleJob(new TriggerKey(triggerName, TriggerExtensions.GetGroup(jobName, jobType, jobGroup)));
        }

        public static bool UnscheduleJob(this IScheduler scheduler, string triggerName, Type jobType, string jobName) {
            return scheduler.UnscheduleJob(triggerName, jobType, jobName, null);
        }

        public static IJobDetail StoreJob(this IScheduler scheduler, IXpandJobDetail xpandJobDetail,ITypesInfo typesInfo) {
            var jobDetail = (JobDetailImpl)(scheduler.GetJobDetail(xpandJobDetail) ?? xpandJobDetail.CreateQuartzJobDetail());
            jobDetail.AssignXpandJobDetail(xpandJobDetail);
            var typeInfo = typesInfo.FindTypeInfo(xpandJobDetail.JobDataMap.GetType());
            jobDetail.AssignDataMap(typeInfo, xpandJobDetail.JobDataMap);
            if (xpandJobDetail.Job.DataMap != null) {
                typeInfo = typesInfo.FindTypeInfo(xpandJobDetail.Job.DataMap.GetType());
                jobDetail.AssignDataMap(typeInfo, xpandJobDetail.Job.DataMap);
            }
            scheduler.StoreJobCore(jobDetail);
            return jobDetail;
        }

        public static void StoreJob(this IScheduler scheduler, IJobDetail jobDetail) {
            scheduler.StoreJobCore((JobDetailImpl)jobDetail);
        }


        static void StoreJobCore(this IScheduler scheduler,JobDetailImpl jobDetail) {
            jobDetail.Durable = true;
            scheduler.AddJob(jobDetail, true);
        }

        public static void StoreJob(this IScheduler scheduler, JobDetailImpl jobDetail) {
            scheduler.StoreJobCore(jobDetail);
        }

        public static void StoreTrigger(this IScheduler scheduler, IOperableTrigger simpleTrigger) {
            scheduler.RescheduleJob(simpleTrigger.Key, simpleTrigger);
        }

        public static JobDetailImpl GetJobDetail(this IScheduler scheduler, string jobDetail, Type jobGroup) {
            return (JobDetailImpl)scheduler.GetJobDetail(new JobKey(jobDetail, jobGroup.FullName));
        }

        public static DateTimeOffset? RescheduleJob(this IScheduler scheduler, IOperableTrigger trigger) {
            return scheduler.RescheduleJob(trigger.Key, trigger);
        }

        public static bool HasTriggers(this IScheduler scheduler, IXpandJobDetail jobDetail) {
            return scheduler.GetTriggersOfJob(jobDetail).Count > 0;
        }

        public static DateTimeOffset ScheduleJob(this IScheduler scheduler, IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName) {
            IOperableTrigger trigger = scheduler.GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            return scheduler.ScheduleJob(trigger);
        }

        public static void StoreTrigger(this IScheduler scheduler, IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName) {
            IOperableTrigger trigger = scheduler.GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            scheduler.StoreTrigger(trigger);
        }

        static IOperableTrigger GetTrigger(this IScheduler scheduler, IXpandJobTrigger jobTrigger, string groupName, string jobName, Type jobType) {
            var trigger = jobTrigger.CreateTrigger(jobName, jobType, groupName);
            CalendarBuilder.Build(jobTrigger, scheduler);
            return trigger;
        }
    }
}
