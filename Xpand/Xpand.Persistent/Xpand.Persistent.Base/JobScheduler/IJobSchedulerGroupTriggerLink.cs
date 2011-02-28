namespace Xpand.Persistent.Base.JobScheduler {
    public interface IJobSchedulerGroupTriggerLink:ISupportSchedulerLink {
        IJobSchedulerGroup JobSchedulerGroup { get; set; }
        IJobTrigger Trigger { get; set; }
    }
}