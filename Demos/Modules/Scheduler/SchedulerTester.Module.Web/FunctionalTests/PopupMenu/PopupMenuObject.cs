using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SchedulerTester.Module.Web.FunctionalTests.PopupMenu {
    [DefaultClassOptions]
    public class PopupMenuObject:Event {
        public PopupMenuObject(Session session) : base(session){
        }
    }
}
