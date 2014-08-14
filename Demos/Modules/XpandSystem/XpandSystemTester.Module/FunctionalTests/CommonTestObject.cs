using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.FunctionalTests {
    [DefaultClassOptions]
    public class CommonTestObject:BaseObject {
        public CommonTestObject(Session session) : base(session){
        }
    }
}
