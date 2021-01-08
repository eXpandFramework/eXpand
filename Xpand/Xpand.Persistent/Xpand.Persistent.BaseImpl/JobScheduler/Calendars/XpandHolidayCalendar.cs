using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Attributes.Custom;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Calendars;
using Xpand.Utils.Helpers;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
    [AdditionalViewControlsRule("XpandHolidayCalendarHelp", "1=1", "1=1", @"Summary:
This implementation of the Calendar stores a list of holidays (full days that are excluded from scheduling). 
Remarks:
The implementation DOES take the year into consideration, so if you want to exclude July 4th for the next 10 years, you need to add 10 entries to the exclude list. ", Position.Top, ViewType = ViewType.DetailView)]
    public class XpandHolidayCalendar : XpandTriggerCalendar, IHolidayCalendar {

        public XpandHolidayCalendar(Session session)
            : base(session) {
        }
        string ITriggerCalendar.CalendarTypeFullName => "Quartz.Impl.Calendar.HolidayCalendar";

        public override void AfterConstruction() {
            base.AfterConstruction();
            _datesExcluded = new List<DateTime>();
        }
        [Persistent("DatesExcluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DateTime> _datesExcluded;

        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDates")]
        [DisplayFormat("{0:dd/MM}")]
        public List<DateTime> DatesExcluded => _datesExcluded;

        [Browsable(false)]
        public List<DateTime> AllDates => DateTimeUtils.GetDates().ToList();
    }
}