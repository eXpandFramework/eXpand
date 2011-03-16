using System.Collections.Generic;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface IMonthlyCalendar : ITriggerCalendar {
        List<int> DaysExcluded { get; }
        List<int> DaysIncluded { get; }
    }
}