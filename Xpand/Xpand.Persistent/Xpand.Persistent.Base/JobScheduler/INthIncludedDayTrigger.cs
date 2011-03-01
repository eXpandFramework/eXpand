using System;
using System.Globalization;
using DevExpress.XtraScheduler;

namespace Xpand.Persistent.Base.JobScheduler {
    public enum NthIncludedDayIntervalType {
        IntervalTypeMonthly = 1,
        IntervalTypeYearly = 2,
        IntervalTypeWeekly = 3
    }

    public interface INthIncludedDayTrigger : IJobTrigger {
        int N { get; set; }
        NthIncludedDayIntervalType IntervalType { get; set; }

        TimeSpan FireAtTime { get; set; }

        int NextFireCutoffInterval { get; set; }
        TimeZoneId TimeZone { get; set; }
        DayOfWeek TriggerCalendarFirstDayOfWeek { get; set; }
        CalendarWeekRule TriggerCalendarWeekRule { get; set; }
    }
}