using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.ExpressApp.AdditionalViewControlsProvider.Logic;
using Xpand.Persistent.Base.JobScheduler.Calendars;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Calendars {
    [AdditionalViewControlsRule("XpandCronCalendarHelp", "1=1", "1=1", @"For example, you could use this calendar to exclude all but business hours (8AM - 5PM) every day using the expression ""* * 0-7,18-23 ? * *"". It is important to remember that the cron expression here describes a set of times to be excluded from firing. Whereas the cron expression in CronTrigger describes a set of times that can be included for firing. Thus, if a CronTrigger has a given cron expression and is associated with a CronCalendar with the same expression, the calendar will exclude all the times the trigger includes, and they will cancel each other out. ", Position.Top, ViewType = ViewType.DetailView)]
    public class XpandCronCalendar : XpandTriggerCalendar, ICronCalendar {
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

        string ITriggerCalendar.CalendarTypeFullName {
            get { return "Quartz.Impl.Calendar.CronCalendar"; }
        }
    }
}