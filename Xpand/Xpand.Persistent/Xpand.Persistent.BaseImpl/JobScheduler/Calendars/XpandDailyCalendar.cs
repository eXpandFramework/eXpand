using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Calendars;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
        [AdditionalViewControlsRule("XpandDailyCalendarHelp", "1=1", "1=1", @"Summary:
This implementation of the Calendar excludes (or includes - see below) a specified time range each day. 
Remarks:
For example, you could use this calendar to exclude business hours (8AM - 5PM) every day. Each DailyCalendar only allows a single time range to be specified, and that time range may not * cross daily boundaries (i.e. you cannot specify a time range from 8PM - 5AM). If the property invertTimeRange is false (default), the time range defines a range of times in which triggers are not allowed to * fire. If invertTimeRange is true, the time range is inverted: that is, all times outside the defined time range are excluded. 
Note when using DailyCalendar, it behaves on the same principals as, for example, WeeklyCalendar defines a set of days that are excluded every week. Likewise, DailyCalendar defines a set of times that are excluded every day. ", Position.Top,ViewType = ViewType.DetailView)]
    public class XpandDailyCalendar : XpandTriggerCalendar, IDailyCalendar {
        public XpandDailyCalendar(Session session)
            : base(session) {
        }
        string ITriggerCalendar.CalendarTypeFullName {
            get { return "Quartz.Impl.Calendar.DailyCalendar"; }
        }
        private bool _invertTimeRange;
        [Tooltip("Indicates whether the time range represents an inverted time range ")]
        public bool InvertTimeRange {
            get {
                return _invertTimeRange;
            }
            set {
                SetPropertyValue("InvertTimeRange", ref _invertTimeRange, value);
            }
        }
        [ProvidedAssociation("XpandDailyCalendar-DateRanges", RelationType.ManyToMany)]
        public XPCollection<XpandDateRange> DateRanges {
            get {
                return GetCollection<XpandDateRange>("DateRanges");
            }
        }

        IList<IDateRange> IDailyCalendar.DateRanges {
            get { return new ListConverter<IDateRange,XpandDateRange>(DateRanges); }
        }
    }
}