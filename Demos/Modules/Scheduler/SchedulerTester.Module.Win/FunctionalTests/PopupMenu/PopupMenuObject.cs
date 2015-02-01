using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using Xpand.ExpressApp.Scheduler.Reminders;

namespace SchedulerTester.Module.Win.FunctionalTests.PopupMenu {
    [DefaultClassOptions]
    public class PopupMenuObject:Event {
        public PopupMenuObject(Session session) : base(session){
        }
    }
}
