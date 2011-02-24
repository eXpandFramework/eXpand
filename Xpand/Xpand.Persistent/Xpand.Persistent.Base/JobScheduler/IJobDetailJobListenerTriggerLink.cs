namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetailJobListenerTriggerLink:ISupportJobDetail {
        IJobListenerTrigger JobListenerTrigger { get; set; }
    }
}