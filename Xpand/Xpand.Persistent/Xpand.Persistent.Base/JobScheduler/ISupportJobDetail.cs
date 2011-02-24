namespace Xpand.Persistent.Base.JobScheduler {
    public interface ISupportJobDetail {
        IJobDetail JobDetail { get; set; }
    }
}