using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace ViewVariantsTester.Module.BusinessObjects {
    [DefaultClassOptions]
    public class TestClass:Person {
        public TestClass(Session session) : base(session) {
        }
    }
}
