namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public interface IJobSchedulerGroupTriggerLink : ISupportSchedulerLink {
        IJobSchedulerGroup JobSchedulerGroup { get; set; }
        IXpandJobTrigger Trigger { get; set; }
    }
}