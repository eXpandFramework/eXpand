using System;
using Quartz;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    public class XpandJobListener : IJobListener {
        public void JobToBeExecuted(IJobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executing);
        }

        public void JobExecutionVetoed(IJobExecutionContext context) {
            TriggerJobs( context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Vetoed);
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {
            TriggerJobs( context.Scheduler, context.JobDetail.JobDataMap, JobListenerEvent.Executed);
        }

        void TriggerJobs(IScheduler scheduler, JobDataMap jobDataMap, JobListenerEvent jobListenerEvent) {
            jobDataMap.GetJobListenerNames(jobListenerEvent).ForEach(TriggerJobsCore(scheduler, jobDataMap));
        }

        Action<JobKey> TriggerJobsCore(IScheduler scheduler, JobDataMap jobDataMap) {
            return key => {
                if (scheduler.GetJobDetail(new JobKey(key.Name, key.Group)) != null)
                    scheduler.TriggerJob(new JobKey(key.Name, key.Group),jobDataMap);
            };
        }

        public string Name {
            get { return typeof(XpandJobListener).Name; }
        }
    }
}
