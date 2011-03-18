using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public interface IXpandScheduler : IScheduler {
        IJobStore JobStore { get; }
        XafApplication Application { get; }
        IJobDetail GetJobDetail(IXpandJobDetail jobDetail);
        void TriggerJob(IXpandJobDetail jobDetail);
        IList<ITrigger> GetTriggersOfJob(IXpandJobDetail jobDetail);
        bool DeleteJob(IXpandJobDetail jobDetail);
        bool UnscheduleJob(string triggerName, Type jobType, string jobName, string jobGroup);
        bool UnscheduleJob(string triggerName, Type jobType, string jobName);
        IJobDetail StoreJob(IXpandJobDetail xpandJobDetail);
        void StoreJob(IJobDetail jobDetail);
        void StoreTrigger(IOperableTrigger simpleTrigger);
        JobDetailImpl GetJobDetail(string jobDetail, Type jobGroup);


        DateTimeOffset? RescheduleJob(IOperableTrigger trigger);
        bool HasTriggers(IXpandJobDetail jobDetail);
        DateTimeOffset ScheduleJob(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName);
        void StoreTrigger(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName);
    }

    public class XpandScheduler : StdScheduler, IXpandScheduler {
        readonly QuartzSchedulerResources _resources;
        public const string TriggerJobListenersOn = "TriggerJobListenersOn";
        public const string TriggerTriggerJobListenersOn = "TriggerTriggerJobListenersOn";

        public XpandScheduler(QuartzScheduler sched, QuartzSchedulerResources resources)
            : base(sched) {
            _resources = resources;
        }


        QuartzSchedulerResources Resources {
            get { return _resources; }
        }

        public IJobStore JobStore {
            get { return Resources.JobStore; }
        }

        public XafApplication Application { get; set; }

        public override void Start() {
            base.Start();
            if (ListenerManager.GetJobListener(typeof(XpandJobListener).Name) == null)
                ListenerManager.AddJobListener(new XpandJobListener());
            if (ListenerManager.GetTriggerListener(typeof(XpandTriggerListener).Name) == null)
                ListenerManager.AddTriggerListener(new XpandTriggerListener());
        }

        public IJobDetail GetJobDetail(IXpandJobDetail jobDetail) {
            return GetJobDetail(jobDetail.Name, jobDetail.Job.JobType);
        }

        public void TriggerJob(IXpandJobDetail jobDetail) {
            TriggerJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public IList<ITrigger> GetTriggersOfJob(IXpandJobDetail jobDetail) {
            return GetTriggersOfJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public virtual bool DeleteJob(IXpandJobDetail jobDetail) {
            return DeleteJob(new JobKey(jobDetail.Name, jobDetail.Job.JobType.FullName));
        }

        public bool UnscheduleJob(string triggerName, Type jobType, string jobName, string jobGroup) {
            return UnscheduleJob(new TriggerKey(triggerName, TriggerExtensions.GetGroup(jobName, jobType, jobGroup)));
        }

        public bool UnscheduleJob(string triggerName, Type jobType, string jobName) {
            return UnscheduleJob(triggerName, jobType, jobName, null);
        }

        public IJobDetail StoreJob(IXpandJobDetail xpandJobDetail) {
            var jobDetail = (JobDetailImpl)(GetJobDetail(xpandJobDetail) ?? xpandJobDetail.CreateQuartzJobDetail());
            jobDetail.AssignXpandJobDetail(xpandJobDetail);
            var typeInfo = Application.ObjectSpaceProvider.TypesInfo.FindTypeInfo(xpandJobDetail.JobDataMap.GetType());
            jobDetail.AssignDataMap(typeInfo, xpandJobDetail.JobDataMap);
            if (xpandJobDetail.Job.DataMap != null) {
                typeInfo = Application.ObjectSpaceProvider.TypesInfo.FindTypeInfo(xpandJobDetail.Job.DataMap.GetType());
                jobDetail.AssignDataMap(typeInfo, xpandJobDetail.Job.DataMap);
            }
            StoreJobCore(jobDetail);
            return jobDetail;
        }

        public void StoreJob(IJobDetail jobDetail) {
            StoreJobCore((JobDetailImpl) jobDetail);
        }


        void StoreJobCore(JobDetailImpl jobDetail) {
            jobDetail.Durable = true;
            Resources.JobStore.StoreJob(jobDetail, true);
        }

        public void StoreJob(JobDetailImpl jobDetail) {
            StoreJobCore(jobDetail);
        }

        public void StoreTrigger(IOperableTrigger simpleTrigger) {
            Resources.JobStore.StoreTrigger(simpleTrigger, true);
        }

        public JobDetailImpl GetJobDetail(string jobDetail, Type jobGroup) {
            return (JobDetailImpl)GetJobDetail(new JobKey(jobDetail, jobGroup.FullName));
        }

        public DateTimeOffset? RescheduleJob(IOperableTrigger trigger) {
            return base.RescheduleJob(trigger.Key, trigger);
        }

        public bool HasTriggers(IXpandJobDetail jobDetail) {
            return GetTriggersOfJob(jobDetail).Count > 0;
        }

        public DateTimeOffset ScheduleJob(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName) {
            IOperableTrigger trigger = GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            return ScheduleJob(trigger);
        }

        public void StoreTrigger(IXpandJobTrigger jobTrigger, IXpandJobDetail jobDetail, string groupName) {
            IOperableTrigger trigger = GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            StoreTrigger(trigger);
        }

        IOperableTrigger GetTrigger(IXpandJobTrigger jobTrigger, string groupName, string jobName, Type jobType) {
            var trigger = jobTrigger.CreateTrigger(jobName, jobType, groupName);
            CalendarBuilder.Build(jobTrigger, this);
            return trigger;
        }
    }
}