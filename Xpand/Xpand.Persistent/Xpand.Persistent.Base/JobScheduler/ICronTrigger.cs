using DevExpress.XtraScheduler;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface ICronTrigger : IJobTrigger {
        string CronExpression { get; set; }

        TimeZoneId TimeZone { get; set; }
    }
}