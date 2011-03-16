using System;
using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface IHolidayCalendar : ITriggerCalendar {
        List<DateTime> DatesExcluded { get; }
    }
}