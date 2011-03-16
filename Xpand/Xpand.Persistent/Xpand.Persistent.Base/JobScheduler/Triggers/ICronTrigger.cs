using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public enum CronTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the CronTrigger wants to be fired now by IScheduler. ")]
        CronTriggerFireOnceNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the CronTrigger wants to have it's next-fire-time updated to the next time in the schedule after the current time (taking into account any associated ICalendar, but it does not want to be fired now.")]
        CronTriggerDoNothing,
    }

    public interface ICronTrigger : IJobTrigger {
        string CronExpression { get; set; }
        TimeZoneId TimeZone { get; set; }
        CronTriggerMisfireInstruction MisfireInstruction { get; set; }
    }
}