namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobDetailJobListenerTriggerLink : ISupportJobDetail {
        IJobListenerTrigger JobListenerTrigger { get; set; }
    }
}