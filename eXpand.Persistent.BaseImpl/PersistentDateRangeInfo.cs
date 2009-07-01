using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace eXpand.Persistent.BaseImpl
{
    [NavigationItem(false)]
    public class PersistentDateRangeInfo : BaseObject {
        public const string DateRangeInfoTypeGroup = "DateRangeInfoTypes";
        public const string DateRangeInfoTypeHoliday = "Holiday";
        public const string DateRangeInfoTypeUndefined = "Undefined";

        private PersistentDateRange dateRange;
        private PersistentEnum dateRangeInfoType;

        public PersistentDateRangeInfo(Session session) : base(session){}

        [Association("DateRange-AllDateRangeInfos")]
        public PersistentDateRange DateRange {
            get {
                return dateRange;
            }
            set {
                SetPropertyValue("DateRange", ref dateRange, value);
            }
        }

        [DataSourceCriteria("Group = '" + DateRangeInfoTypeGroup + "'")]
        public PersistentEnum DateRangeInfoType {
            get { return dateRangeInfoType; }
            set { SetPropertyValue("DateRangeInfoType", ref dateRangeInfoType, value); }
        }

        protected virtual void AssignDefaultEnumType() {
            DateRangeInfoType = GetDateRangeInfoTypeEnumByValue(Session, DateRangeInfoTypeUndefined);
        }

        public static PersistentEnum GetDateRangeInfoTypeEnumByValue(Session session, string enumValue) {
            return PersistentEnum.GetEnumByGroupAndValue(session, DateRangeInfoTypeGroup, enumValue);
        }

        [Association("PersistentDateRangeInfo-RelatedEvents")]
        public XPCollection<SchedulerEvent> RelatedEvents {
            get {
                return GetCollection<SchedulerEvent>("RelatedEvents");
            }
        }

        public void SynchEvents() {SynchEvents(true);}

        public void SynchEvents(bool clearExistingEvents){
            RelatedEvents.SuspendChangedEvents();
            if (clearExistingEvents){
                Session.Delete(RelatedEvents);
            }
            ProcessRelatedEvents();
            RelatedEvents.ResumeChangedEvents();
            Save();
        }

        protected virtual void ProcessRelatedEvents() { }
    }
}