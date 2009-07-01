using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using eXpand.ExpressApp.Scheduler.BaseObjects.Ranges;
using eXpand.Persistent.Base.Interfaces;
using eXpand.Xpo.Converters.ValueConverters;

namespace eXpand.Persistent.BaseImpl
{
    [NavigationItem(false)]
    [DefaultProperty("Name")]
    public class PersistentDateRange : BaseObject, INamedObject, IDateRange, IHasResources {
        [Persistent("DateRange"), Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        [ValueConverter(typeof(SerializableObjectConverter))]
        protected DateRange dateRange;

        private PersistentEnum dateRangeType;
        public ResourceHelper ResourceHelper { get; private set; }
        private int indexer;
        private string name;
        private int year;

        public PersistentDateRange(Session session) : base(session){
            ResourceHelper = new ResourceHelper(this);
        }

        #region IDateRange Members
        public DateTime? FirstDay {
            get { return dateRange.FirstDay; }
            set {
                dateRange.FirstDay = (value.HasValue ? value.Value.Date : value);
                OnChanged("FirstDay");
            }
        }

        public DateTime? LastDay {
            get { return dateRange.LastDay; }
            set {
                dateRange.LastDay = (value.HasValue ? value.Value.Date : value);
                OnChanged("LastDay");
            }
        }

        [NonPersistent]
        public DateTime? LastMinute {
            get { return dateRange.LastMinute; }
        }

        [Persistent]
        public bool IsSingleDay {
            get { return dateRange.IsSingleDay; }
        }

        [Persistent]
        public bool IsEmpty {
            get { return dateRange.IsEmpty; }
        }

        public int NumberOfDays {
            get { return dateRange.NumberOfDays; }
        }
        #endregion
        #region INamedObject Members
        public string Name {
            get { return name; }
            set { SetPropertyValue("Name", ref name, value); }
        }
        #endregion
        
        [Association("DateRange-AllDateRangeInfos")]
        public XPCollection<PersistentDateRangeInfo> AllDateRangeInfos {
            get {
                return GetCollection<PersistentDateRangeInfo>("AllDateRangeInfos");
            }
        }

        [DataSourceCriteria("Group = '" + DateRangeTypeGroup + "'")]
        public PersistentEnum DateRangeType {
            get { return dateRangeType; }
            set { SetPropertyValue("DateRangeInfoType", ref dateRangeType, value); }
        }

        public int Year {
            get { return year; }
            set { SetPropertyValue("Year", ref year, value); }
        }

        public List<SchedulerEvent> Events {
            get { return AllDateRangeInfos.SelectMany(info=>info.RelatedEvents).ToList(); }
        }

        public int Indexer {
            get { return indexer; }
            set { SetPropertyValue("Indexer", ref indexer, value); }
        }
        
        public override void AfterConstruction() {
            base.AfterConstruction();
            dateRange = new DateRange();
            AssignDefaultEnumType();
        }

        protected virtual void AssignDefaultEnumType() {
            DateRangeType = GetDateRangeTypeEnumByValue(Session, DateRangeTypeUndefined);
        }

        //public XPCollection GetDateRangeInfosOfType<TDateRangeInfo>() {
        //    return new XPCollection(
        //                this.Session
        //                , ((XPClassInfo) XafTypesInfo.Instance.FindTypeInfo(typeof(TDateRangeInfo)))
        //                , AllDateRangeInfos.OfType<TDateRangeInfo>());
        //}

        public XPCollection<TDateRangeInfo> GetDateRangeInfosOfType<TDateRangeInfo>() {
            return new XPCollection<TDateRangeInfo>(
                Session, AllDateRangeInfos.OfType<TDateRangeInfo>());
        }

        public static PersistentEnum GetDateRangeTypeEnumByValue(Session session, string enumValue) {
            return PersistentEnum.GetEnumByGroupAndValue(session, DateRangeTypeGroup, enumValue);
        }

        public const string DateRangeTypeCalendarWeek = "CalendarWeek";
        public const string DateRangeTypeGroup = "DateRangeTypes";
        public const string DateRangeTypeUndefined = "Undefined";

        #region FullName Implementation

        //protected override void OnSaving() {
        //    if (String.IsNullOrEmpty(Name)) {
        //        SetDefaultDateRangeName();
        //    }
        //    RefreshResourceBindings();
        //    base.OnSaving();
        //}

        //private const string defaultNameFormatForSingleDay = "{FirstDayValue}";
        //private static string defaultNameFormatForMultiDay = defaultNameFormatForSingleDay + " - {LastDayValue}";
        //private const string defaultNameFormatForCalendarWeek = "{nameFormatWeekName} ({nameFormatFirstDay} - {nameFormatLastDay} ) / {Year}";

        //private string nameFormat;
        //public string NameFormat {
        //    get {
        //        return nameFormat;
        //    }
        //    set {
        //        SetPropertyValue("NameFormat", ref nameFormat, value);
        //    }
        //}

        //protected virtual void SetDefaultDateRangeName() {
        //    if (!string.IsNullOrEmpty(nameFormat)) {
        //        Name = ObjectFormatter.Format(nameFormat, this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
        //    } else if (!IsEmpty) {
        //        string defaultFormat = IsSingleDay ? defaultNameFormatForSingleDay : defaultNameFormatForMultiDay;
        //        if (DateRangeInfoType == GetDateRangeTypeEnumByValue(this.Session, PersistentDateRange.DateRangeTypeCalendarWeek)){
        //            defaultFormat = defaultNameFormatForCalendarWeek;    
        //        }

        //        Name = ObjectFormatter.Format(defaultFormat, this, EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
        //        NameFormat = defaultFormat;
        //    }
        //}

        //protected DateTime LastDayValue {
        //    get {
        //        return LastDay.Value;
        //    }
        //}

        //protected DateTime FirstDayValue {
        //    get {
        //        return FirstDay.Value;
        //    }
        //}

        //protected string nameFormatWeekName {
        //    get {
        //        return string.Format("Week {0}", indexer);
        //    }
        //}

        //protected string nameFormatFirstDay {
        //    get {
        //        return string.Format("{0}/{1}", FirstDayValue.Day, FirstDayValue.Month);

        //    }
        //}

        //protected string nameFormatLastDay {
        //    get {
        //        return string.Format("{0}/{1}", LastDayValue.Day, LastDayValue.Month);

        //    }
        //}
        #endregion
    }
}