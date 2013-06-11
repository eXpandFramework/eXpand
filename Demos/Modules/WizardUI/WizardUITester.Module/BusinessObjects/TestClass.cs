using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace WizardUITester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestClass:BaseObject {
        public TestClass(Session session) : base(session) {
        }
    }
}
