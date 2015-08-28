using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace WorkflowTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class DemoTask:Task {
        public DemoTask(Session session) : base(session){
        }
    }
}
