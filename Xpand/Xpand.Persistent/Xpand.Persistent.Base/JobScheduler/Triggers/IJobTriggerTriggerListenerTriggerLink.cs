namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobTriggerTriggerListenerTriggerLink {
        IXpandJobTrigger JobTrigger { get; set; }
        ITriggerListenerTrigger TriggerListenerTrigger { get; set; }
    }
}