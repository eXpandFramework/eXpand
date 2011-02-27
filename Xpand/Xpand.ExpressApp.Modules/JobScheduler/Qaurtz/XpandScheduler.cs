using System;
using Quartz;
using Quartz.Core;
using Quartz.Impl;
using Quartz.Spi;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    
    public class XpandScheduler : StdScheduler {
        readonly QuartzSchedulerResources _resources;
        readonly SchedulingContext _schedulingContext;
        public const string TriggerJobListenersOn = "TriggerJobListenersOn";

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

        public override void Start() {
            base.Start();
            if (GetJobListener(typeof(XpandJobListener).Name)==null)
                AddJobListener(new XpandJobListener());
        }

        public JobDetail GetJobDetail(IJobDetail jobDetail) {
            return GetJobDetail(jobDetail.Name, jobDetail.JobType) ;
        }

        public void TriggerJob(IJobDetail jobDetail) {
            TriggerJob(jobDetail.Name, jobDetail.JobType.FullName);
        }

        public Trigger[] GetTriggersOfJob(IJobDetail jobDetail) {
            return GetTriggersOfJob(jobDetail.Name, jobDetail.JobType.FullName);
        }

        public virtual bool DeleteJob(IJobDetail jobDetail) {
            return DeleteJob(jobDetail.Name, jobDetail.JobType.FullName);
        }

        public bool UnscheduleJob(string triggerName, Type jobType) {
            return UnscheduleJob(triggerName, jobType.FullName);
        }

        public void StoreJob(IJobDetail xpandJobDetail) {
            var jobDetail = GetJobDetail(xpandJobDetail) ?? Mapper.CreateJobDetail(xpandJobDetail);
            Mapper.AssignJobDetail(jobDetail, xpandJobDetail);
            jobDetail.Durable = true;
            Resources.JobStore.StoreJob(SchedulingContext, jobDetail, true);
        }

        public void StoreJob(JobDetail jobDetail) {
            Resources.JobStore.StoreJob(SchedulingContext, jobDetail, true);
        }

        public void StoreTrigger(SimpleTrigger simpleTrigger) {
            Resources.JobStore.StoreTrigger(SchedulingContext, simpleTrigger, true);
        }

        public JobDetail GetJobDetail(string jobDetail, Type jobGroup) {
            return GetJobDetail(jobDetail, jobGroup.FullName);
        }
    }
}