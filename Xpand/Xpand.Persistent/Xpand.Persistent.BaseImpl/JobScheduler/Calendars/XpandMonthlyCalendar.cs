using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Quartz.Impl.Calendar;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Calendars;
using Xpand.Xpo.Converters.ValueConverters;

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
        public List<int> DaysExcluded {
            get { return _daysExcluded; }
        }
        [Persistent("DaysIncluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<int> _daysIncluded = new List<int>();
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDays")]
        public List<int> DaysIncluded {
            get { return _daysIncluded; }
        }
        [Browsable(false)]
        public List<int> AllDays {
            get {
                return Enumerable.Range(1, 31).ToList();
            }
        }

        Type ITriggerCalendar.CalendarType {
            get { return typeof(MonthlyCalendar); }
        }

    }
}