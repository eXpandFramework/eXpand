using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface ISupportJobDetail : ISupportSchedulerLink {
        IJobDetail JobDetail { get; set; }
    }
}