using DevExpress.Xpo;
using TimeZoneId = Xpand.Persistent.Base.JobScheduler.Triggers.TimeZoneId;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface ITriggerCalendar {
        string Name { get; set; }
        TimeZoneId TimeZone { get; set; }

        [Size(SizeAttribute.Unlimited)]
        string Description { get; set; }

        string CalendarTypeFullName { get; }
    }
}