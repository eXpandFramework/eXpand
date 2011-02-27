using System;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.JobScheduler {
    public interface ISimpleTrigger : IJobTrigger {
        void SetFinalFireTimeUtc(DateTime? dateTime);
        SimpleTriggerMisfireInstruction MisfireInstruction { get; set; }

        int? RepeatCount { get; set; }


        TimeSpan? RepeatInterval { get; set; }

        int TimesTriggered { get; set; }

        DateTime? FinalFireTimeUtc { get; }
        
    }
    public enum SimpleTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be fired now by IScheduler. 
NOTE: This instruction should typically only be used for 'one-shot' (non-repeating) Triggers. If it is used on a trigger with a repeat count > 0 then it is equivalent to the instruction RescheduleNowWithRemainingRepeatCount . 
")]
        FireNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to 'now' (even if the associated ICalendar excludes 'now') with the repeat count left as-is. This does obey the Trigger end-time however, so if 'now' is after the end-time the Trigger will not fire again. 
Remarks:
NOTE: Use of this instruction causes the trigger to 'forget' the start-time and repeat-count that it was originally setup with (this is only an issue if you for some reason wanted to be able to tell what the original values were at some later time). 
")]
        RescheduleNowWithExistingRepeatCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to 'now' (even if the associated ICalendar excludes 'now') with the repeat count set to what it would be, if it had not missed any firings. This does obey the Trigger end-time however, so if 'now' is after the end-time the Trigger will not fire again. 
NOTE: Use of this instruction causes the trigger to 'forget' the start-time and repeat-count that it was originally setup with. Instead, the repeat count on the trigger will be changed to whatever the remaining repeat count is (this is only an issue if you for some reason wanted to be able to tell what the original values were at some later time). 

NOTE: This instruction could cause the Trigger to go to the 'COMPLETE' state after firing 'now', if all the repeat-fire-times where missed. 
")]
        RescheduleNowWithRemainingRepeatCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to the next scheduled time after 'now' - taking into account any associated ICalendar, and with the repeat count set to what it would be, if it had not missed any firings. 
Remarks:
NOTE/WARNING: This instruction could cause the Trigger to go directly to the 'COMPLETE' state if all fire-times where missed. ")]
        RescheduleNextWithRemainingCount,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the SimpleTrigger wants to be re-scheduled to the next scheduled time after 'now' - taking into account any associated ICalendar, and with the repeat count left unchanged. 
Remarks:
NOTE/WARNING: This instruction could cause the Trigger to go directly to the 'COMPLETE' state if all the end-time of the trigger has arrived.
")]
        RescheduleNextWithExistingCount,

    }

}