using System.Collections.Generic;
using Xpand.Persistent.Base.General;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface IDailyCalendar : ITriggerCalendar {
        bool InvertTimeRange { get; set; }
        IList<IDateRange> DateRanges { get; }
    }
}