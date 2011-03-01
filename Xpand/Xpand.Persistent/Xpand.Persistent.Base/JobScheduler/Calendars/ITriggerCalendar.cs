using System;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;

namespace Xpand.Persistent.Base.JobScheduler.Calendars {
    public interface ITriggerCalendar {
        string Name { get; set; }
        TimeZoneId TimeZone { get; set; }

        [Size(SizeAttribute.Unlimited)]
        string Description { get; set; }
        Type CalendarType { get; }
    }
}