using System;
using DevExpress.ExpressApp;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public interface IXpandScheduler : IScheduler {
        SchedulingContext SchedulingContext { get; }
        IJobStore JobStore { get; }
        XafApplication Application { get; }
        JobDetail GetJobDetail(IJobDetail jobDetail);
        void TriggerJob(IJobDetail jobDetail);
        Trigger[] GetTriggersOfJob(IJobDetail jobDetail);
        bool DeleteJob(IJobDetail jobDetail);
        bool UnscheduleJob(string triggerName, Type jobType, string jobName, string jobGroup);
        bool UnscheduleJob(string triggerName, Type jobType, string jobName);
        JobDetail StoreJob(IJobDetail xpandJobDetail);
        void StoreJob(JobDetail jobDetail);
        void StoreTrigger(Trigger simpleTrigger);
        JobDetail GetJobDetail(string jobDetail, Type jobGroup);


        DateTime? RescheduleJob(Trigger trigger);
        bool HasTriggers(IJobDetail jobDetail);
        DateTime ScheduleJob(IJobTrigger jobTrigger, IJobDetail jobDetail, string groupName);
        void StoreTrigger(IJobTrigger jobTrigger, IJobDetail jobDetail, string groupName);
    }

    public class XpandScheduler : StdScheduler, IXpandScheduler {
        readonly QuartzSchedulerResources _resources;
        readonly SchedulingContext _schedulingContext;
        public const string TriggerJobListenersOn = "TriggerJobListenersOn";
        public const string TriggerTriggerJobListenersOn = "TriggerTriggerJobListenersOn";

        public XpandScheduler(QuartzScheduler sched, SchedulingContext schedCtxt, QuartzSchedulerResources resources, SchedulingContext schedulingContext)
            : base(sched, schedCtxt) {
            _resources = resources;
            _schedulingContext = schedulingContext;
        }

        public SchedulingContext SchedulingContext {
            get { return _schedulingContext; }
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
            if (GetJobListener(typeof(XpandJobListener).Name) == null)
                AddJobListener(new XpandJobListener());
            if (GetTriggerListener(typeof(XpandTriggerListener).Name) == null)
                AddTriggerListener(new XpandTriggerListener());
        }

        public JobDetail GetJobDetail(IJobDetail jobDetail) {
            return GetJobDetail(jobDetail.Name, jobDetail.Job.JobType);
        }

        public void TriggerJob(IJobDetail jobDetail) {
            TriggerJob(jobDetail.Name, jobDetail.Job.JobType.FullName);
        }

        public Trigger[] GetTriggersOfJob(IJobDetail jobDetail) {
            return GetTriggersOfJob(jobDetail.Name, jobDetail.Job.JobType.FullName);
        }

        public virtual bool DeleteJob(IJobDetail jobDetail) {
            return DeleteJob(jobDetail.Name, jobDetail.Job.JobType.FullName);
        }

        public bool UnscheduleJob(string triggerName, Type jobType, string jobName, string jobGroup) {
            return UnscheduleJob(triggerName, TriggerExtensions.GetGroup(jobName, jobType, jobGroup));
        }

        public bool UnscheduleJob(string triggerName, Type jobType, string jobName) {
            return UnscheduleJob(triggerName, jobType, jobName, null);
        }

        public JobDetail StoreJob(IJobDetail xpandJobDetail) {
            var jobDetail = GetJobDetail(xpandJobDetail) ?? xpandJobDetail.CreateQuartzJobDetail();
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

        void StoreJobCore(JobDetail jobDetail) {
            jobDetail.Durable = true;
            Resources.JobStore.StoreJob(SchedulingContext, jobDetail, true);
        }

        public void StoreJob(JobDetail jobDetail) {
            StoreJobCore(jobDetail);
        }

        public void StoreTrigger(Trigger simpleTrigger) {
            Resources.JobStore.StoreTrigger(SchedulingContext, simpleTrigger, true);
        }

        public JobDetail GetJobDetail(string jobDetail, Type jobGroup) {
            return GetJobDetail(jobDetail, jobGroup.FullName);
        }

        public DateTime? RescheduleJob(Trigger trigger) {
            return base.RescheduleJob(trigger.Name, trigger.Group, trigger);
        }

        public bool HasTriggers(IJobDetail jobDetail) {
            return GetTriggersOfJob(jobDetail).Length > 0;
        }

        public DateTime ScheduleJob(IJobTrigger jobTrigger, IJobDetail jobDetail, string groupName) {
            Trigger trigger = GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            return ScheduleJob(trigger);
        }

        public void StoreTrigger(IJobTrigger jobTrigger, IJobDetail jobDetail, string groupName) {
            Trigger trigger = GetTrigger(jobTrigger, groupName, jobDetail.Name, jobDetail.Job.JobType);
            StoreTrigger(trigger);
        }

        Trigger GetTrigger(IJobTrigger jobTrigger, string groupName, string jobName, Type jobType) {
            var trigger = jobTrigger.CreateTrigger(jobName, jobType, groupName);
            CalendarBuilder.Build(jobTrigger, this);
            return trigger;
        }
    }
}