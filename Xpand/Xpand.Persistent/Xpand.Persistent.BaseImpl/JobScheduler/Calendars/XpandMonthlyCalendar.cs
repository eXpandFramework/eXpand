using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Extensions.XAF.Xpo.ValueConverters;
using Xpand.Persistent.Base.AdditionalViewControls;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Calendars;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
    [AdditionalViewControlsRule("XpandMonthlyCalendarHelp", "1=1", "1=1",
        @"This implementation of the Calendar excludes a set of days of the month. You may use it to exclude every 1. of each month for example. But you may define any day of a month. "
        , Position.Top, ViewType = ViewType.DetailView)]
    public class XpandMonthlyCalendar : XpandTriggerCalendar, IMonthlyCalendar {
        public XpandMonthlyCalendar(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            _daysExcluded = new List<int>();
            _daysIncluded = new List<int>();
        }

        [Persistent("DaysExcluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<int> _daysExcluded = new List<int>();
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDays")]
        public List<int> DaysExcluded => _daysExcluded;

        [Persistent("DaysIncluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<int> _daysIncluded = new List<int>();
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDays")]
        public List<int> DaysIncluded => _daysIncluded;

        [Browsable(false)]
        public List<int> AllDays => Enumerable.Range(1, 31).ToList();

        string ITriggerCalendar.CalendarTypeFullName => "Quartz.Impl.Calendar.MonthlyCalendar";
    }
}