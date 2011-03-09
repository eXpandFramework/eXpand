using System;
using Quartz;
using Quartz.Util;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    public class XpandTriggerListener : ITriggerListener {
        

        public string Name {
            get { return typeof(XpandTriggerListener).Name; }
        }

        public void TriggerFired(Trigger trigger, JobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Fired);
        }

        public bool VetoJobExecution(Trigger trigger, JobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Vetoed);
            return false;
        }

        void ITriggerListener.TriggerMisfired(Trigger trigger) {
            
        }

        public void TriggerComplete(Trigger trigger, JobExecutionContext context,SchedulerInstruction triggerInstructionCode) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Complete);
        }
        void TriggerJobs(IScheduler scheduler, JobDataMap jobDataMap, TriggerListenerEvent jobListenerEvent) {
            jobDataMap.GetTriggerListenerNames(jobListenerEvent).ForEach(TriggerJobsCore(scheduler));
        }

        Action<Key> TriggerJobsCore(IScheduler scheduler) {
            return key => scheduler.TriggerJob(key.Name, key.Group);
        }

    }
}