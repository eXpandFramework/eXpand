using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface IAnnualCalendar : ITriggerCalendar {
        List<DateTime> DatesExcluded { get; }
        List<DateTime> DatesIncluded { get; }
    }
}