using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Scheduler.Reminders;

namespace SchedulerTester.Module.Win.FunctionalTests.Reminder {
    [DefaultClassOptions]
    [SupportsReminder]
    public class ReminderObject:Event {
        public ReminderObject(Session session) : base(session){
        }
    }
}
