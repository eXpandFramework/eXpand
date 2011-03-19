using Quartz;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.ExpressApp.JobScheduler.QuartzExtensions {
    ///<summary>
    ///</summary>
    public class XpandTriggerListener : ITriggerListener {
        

        public string Name {
            get { return typeof(XpandTriggerListener).Name; }
        }

        public void TriggerFired(ITrigger trigger, IJobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Fired);
        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Vetoed);
            return false;
        }

        void ITriggerListener.TriggerMisfired(ITrigger trigger) {
            
        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context,SchedulerInstruction triggerInstructionCode) {
            TriggerJobs(context.Scheduler, context.JobDetail.JobDataMap, TriggerListenerEvent.Complete);
        }
        void TriggerJobs(IScheduler scheduler, JobDataMap jobDataMap, TriggerListenerEvent jobListenerEvent) {
            jobDataMap.GetTriggerListenerNames(jobListenerEvent).ForEach(scheduler.TriggerJob);
        }



    }
}