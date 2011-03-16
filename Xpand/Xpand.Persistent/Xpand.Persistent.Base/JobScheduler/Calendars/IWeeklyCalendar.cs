using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface IWeeklyCalendar : ITriggerCalendar {
        List<DayOfWeek> DaysOfWeekExcluded { get; }
        List<DayOfWeek> DaysOfWeekIncluded { get; }
    }
}