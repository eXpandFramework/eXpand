using System;
using System.Globalization;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.Persistent.Base.JobScheduler.Triggers {
    public enum NthIncludedDayIntervalType {
        IntervalTypeMonthly = 1,
        IntervalTypeYearly = 2,
        IntervalTypeWeekly = 3
    }
    public enum NthIncludedDayTriggerMisfireInstruction {
        InstructionNotSet,
        SmartPolicy,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the NthIncludedDayTrigger wants to be fired now by the IScheduler")]
        NthIncludedDayTriggerFireOnceNow,
        [Tooltip(@"Instructs the IScheduler that upon a mis-fire situation, the NthIncludedDayTrigger wants to have nextFireTime updated to the next time in the schedule after the current time, but it does not want to be fired now. ")]
        NthIncludedDayDoNothing,
    }

    public interface INthIncludedDayTrigger : IXpandJobTrigger {
        int N { get; set; }
        NthIncludedDayIntervalType IntervalType { get; set; }

        TimeSpan FireAtTime { get; set; }

        int NextFireCutoffInterval { get; set; }
        TimeZoneId TimeZone { get; set; }
        DayOfWeek TriggerCalendarFirstDayOfWeek { get; set; }
        CalendarWeekRule TriggerCalendarWeekRule { get; set; }
        NthIncludedDayTriggerMisfireInstruction MisfireInstruction { get; set; }
    }
}