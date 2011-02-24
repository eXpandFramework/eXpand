namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobDetailTriggerLink : ISupportJobDetail {
        IJobTrigger JobTrigger { get; set; }
    }
}