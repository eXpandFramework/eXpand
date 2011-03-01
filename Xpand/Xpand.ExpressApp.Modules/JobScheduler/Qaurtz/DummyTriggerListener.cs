using Quartz;

namespace Xpand.ExpressApp.JobScheduler.Qaurtz {
    internal class DummyTriggerListener : ITriggerListener {
        public string Name {
            get { return GetType().FullName; }
        }

        public void TriggerFired(Trigger trigger, JobExecutionContext context) {
        }

        public bool VetoJobExecution(Trigger trigger, JobExecutionContext context) {
            return false;
        }

        public void TriggerMisfired(Trigger trigger) {
        }

        public void TriggerComplete(Trigger trigger, JobExecutionContext context,
                                    SchedulerInstruction triggerInstructionCode) {
        }
    }
}