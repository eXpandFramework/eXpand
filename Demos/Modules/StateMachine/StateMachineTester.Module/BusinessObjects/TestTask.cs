using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace StateMachineTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestTask:Task {
        public TestTask(Session session) : base(session) {
        }
    }
}
