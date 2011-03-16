namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobTriggerTriggerListenerTriggerLink {
        IJobTrigger JobTrigger { get; set; }
        ITriggerListenerTrigger TriggerListenerTrigger { get; set; }
    }
}