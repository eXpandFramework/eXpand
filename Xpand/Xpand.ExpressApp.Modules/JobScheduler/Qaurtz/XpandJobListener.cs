using System;
using Quartz;
using Quartz.Util;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandJobListener : IJobListener {
        readonly JobDataMapKeyCalculator jobDataMapKeyCalculator = new JobDataMapKeyCalculator();
        public void JobToBeExecuted(JobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executing);
        }

        public void JobExecutionVetoed(JobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Vetoed);
        }

        public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executed);
        }

        void TriggerJobs(IScheduler scheduler, JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            jobDataMapKeyCalculator.GetJobListenerNames(jobDataMap, jobListenerEvent).ForEach(TriggerJobsCore(scheduler));
        }

        Action<Key> TriggerJobsCore(IScheduler scheduler) {
            return key => scheduler.TriggerJob(key.Name, key.Group);
        }

        public string Name {
            get { return typeof(XpandJobListener).Name; }
        }
    }
}
