using System;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Quartz.Impl.Calendar;
using Xpand.Persistent.Base.General.CustomAttributes;
using Xpand.Persistent.Base.JobScheduler.Calendars;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
    [Tooltip(@"For example, you could use this calendar to exclude all but business hours (8AM - 5PM) every day using the expression ""* * 0-7,18-23 ? * *"". It is important to remember that the cron expression here describes a set of times to be excluded from firing. Whereas the cron expression in CronTrigger describes a set of times that can be included for firing. Thus, if a CronTrigger has a given cron expression and is associated with a CronCalendar with the same expression, the calendar will exclude all the times the trigger includes, and they will cancel each other out. ")]
    public class XpandCronCalendar : XpandTriggerCalendar, ICronCalendar, ITriggerCalendar {
        public XpandCronCalendar(Session session)
            : base(session) {
        }
        private string _cronExpression;
        [RuleRequiredField]
        public string CronExpression {
            get {
                return _cronExpression;
            }
            set {
                SetPropertyValue("CronExpression", ref _cronExpression, value);
            }
        }

        Type ITriggerCalendar.CalendarType {
            get { return typeof(CronCalendar); }
        }
    }
}