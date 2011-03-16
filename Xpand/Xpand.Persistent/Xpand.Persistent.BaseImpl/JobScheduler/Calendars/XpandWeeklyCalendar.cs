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
    [AdditionalViewControlsRule("XpandWeeklyCalendarHelp", "1=1", "1=1",
        @"This implementation of the Calendar excludes a set of days of the week. You may use it to exclude weekends for example. But you may define any day of the week.", Position.Top, ViewType = ViewType.DetailView)]
    public class XpandWeeklyCalendar : XpandTriggerCalendar, IWeeklyCalendar {
        public XpandWeeklyCalendar(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            _daysExcluded = new List<DayOfWeek> { DayOfWeek.Sunday, DayOfWeek.Saturday };
            _daysIncluded = new List<DayOfWeek>();
        }

        [Persistent("DaysOfWeekExcluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DayOfWeek> _daysExcluded;
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDaysOfWeek")]
        public List<DayOfWeek> DaysOfWeekExcluded {
            get { return _daysExcluded; }
        }
        [Persistent("DaysOfWeekIncluded")]
        [Size(SizeAttribute.Unlimited)]
        [ValueConverter(typeof(SerializableObjectConverter))]
        private List<DayOfWeek> _daysIncluded;
        [PropertyEditor(typeof(IChooseFromListCollectionEditor))]
        [DataSourceProperty("AllDaysOfWeek")]
        public List<DayOfWeek> DaysOfWeekIncluded {
            get { return _daysIncluded; }
        }
        [Browsable(false)]
        public List<DayOfWeek> AllDaysOfWeek {
            get {
                return Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            }
        }

        Type ITriggerCalendar.CalendarType {
            get { return typeof(WeeklyCalendar); }
        }

    }
}