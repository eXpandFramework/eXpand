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
    [AdditionalViewControlsRule("XpandAnnualCalendarHelp", "1=1", "1=1", @"This implementation of the Calendar excludes a set of days of the year. You may use it to exclude bank holidays which are on the same date every year. ", Position.Top, ViewType = ViewType.DetailView)]
    public class XpandAnnualCalendar : XpandTriggerCalendar, IAnnualCalendar {
        public XpandAnnualCalendar(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            _datesExcluded = new List<DateTime>();
            _datesIncluded = new List<DateTime>();
        }
        [Persistent("DatesExcluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DateTime> _datesExcluded;

        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDates")]
        [DisplayFormat("{0:dd/MM}")]
        public List<DateTime> DatesExcluded => _datesExcluded;

        [Persistent("DatesIncluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DateTime> _datesIncluded;

        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDates")]
        [DisplayFormat("{0:dd/MM}")]
        public List<DateTime> DatesIncluded => _datesIncluded;

        [Browsable(false)]
        public List<DateTime> AllDates => DateTimeUtils.GetDates().ToList();

        string ITriggerCalendar.CalendarTypeFullName => "Quartz.Impl.Calendar.AnnualCalendar";
    }
}