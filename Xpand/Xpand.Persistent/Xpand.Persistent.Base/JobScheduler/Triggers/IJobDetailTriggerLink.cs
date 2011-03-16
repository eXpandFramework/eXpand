namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobDetailTriggerLink : ISupportJobDetail {
        IJobTrigger JobTrigger { get; set; }
    }
}