using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.JobScheduler.Calendars;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
    public abstract class XpandTriggerCalendar : XpandCustomObject, ITriggerCalendar {
        protected XpandTriggerCalendar(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            TimeZone = RegistryTimeZoneProvider.GetTimeZoneIdByRegistryKeyName(System.TimeZone.CurrentTimeZone.StandardName);
        }
        private string _name;
        [RuleRequiredField]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
        private TimeZoneId _timeZone;
        [RuleRequiredField]
        public TimeZoneId TimeZone {
            get {
                return _timeZone;
            }
            set {
                SetPropertyValue("TimeZone", ref _timeZone, value);
            }
        }
        private string _description;
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get {
                return _description;
            }
            set {
                SetPropertyValue("Description", ref _description, value);
            }
        }

        string ITriggerCalendar.CalendarTypeFullName {
            get { throw new NotImplementedException(); }
        }
    }
}
