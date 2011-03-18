namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobDetailTriggerLink : ISupportJobDetail {
        IXpandJobTrigger JobTrigger { get; set; }
    }
}