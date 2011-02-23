namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetailTriggerLink {
        IJobDetail JobDetail { get; set; }
        IJobTrigger JobTrigger { get; set; }
    }
}