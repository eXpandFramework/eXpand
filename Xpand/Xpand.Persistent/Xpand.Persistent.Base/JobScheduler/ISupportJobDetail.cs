namespace Xpand.Persistent.Base.JobScheduler {
    public interface ISupportJobDetail:ISupportSchedulerLink {
        IJobDetail JobDetail { get; set; }
    }
}