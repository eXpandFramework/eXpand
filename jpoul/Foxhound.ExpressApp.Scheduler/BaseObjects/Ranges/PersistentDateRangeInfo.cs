using System;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Foxhound.ExpressApp.Scheduler.BaseObjects.Events;
using Foxhound.ExpressApp.Taxonomy.BaseObjects;

namespace Foxhound.ExpressApp.Scheduler.BaseObjects.Ranges{
    [NavigationItem(false)]
    public class PersistentDateRangeInfo : BaseObject {
        public const string DateRangeInfoTypeGroup = "DateRangeInfoTypes";
        public const string DateRangeInfoTypeHoliday = "Holiday";
        public const string DateRangeInfoTypeUndefined = "Undefined";

        private PersistentDateRange dateRange;
        private Term dateRangeInfoType;

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
        public Term DateRangeInfoType {
            get { return dateRangeInfoType; }
            set { SetPropertyValue("DateRangeInfoType", ref dateRangeInfoType, value); }
        }

        protected virtual void AssignDefaultEnumType() {
            DateRangeInfoType = GetDateRangeInfoTypeEnumByValue(this.Session, DateRangeInfoTypeUndefined);
        }

        public static Term GetDateRangeInfoTypeEnumByValue(Session session, string enumValue) {
            throw new NotImplementedException();
            //return PersistentEnum.GetEnumByGroupAndValue(session, DateRangeInfoTypeGroup, enumValue);
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
                this.Session.Delete(RelatedEvents);
            }
            ProcessRelatedEvents();
            RelatedEvents.ResumeChangedEvents();
            Save();
        }

        protected virtual void ProcessRelatedEvents() { }
    }
}