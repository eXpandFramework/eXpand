using System;
using Quartz;
using Quartz.Util;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandJobListener : IJobListener {
        public void JobToBeExecuted(JobExecutionContext context) {
            TriggerJobs((IXpandScheduler) context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executing);
        }

        public void JobExecutionVetoed(JobExecutionContext context) {
            TriggerJobs((IXpandScheduler) context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Vetoed);
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            TriggerJobs((IXpandScheduler) context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executed);
        }

        void TriggerJobs(IXpandScheduler scheduler, JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            jobDataMap.GetJobListenerNames(jobListenerEvent).ForEach(TriggerJobsCore(scheduler, jobDataMap));
        }

        Action<Key> TriggerJobsCore(IXpandScheduler scheduler, JobDataMap jobDataMap) {
            return key => scheduler.TriggerJob(key.Name, key.Group,jobDataMap);
        }

        public string Name {
            get { return typeof(XpandJobListener).Name; }
        }
    }
}
