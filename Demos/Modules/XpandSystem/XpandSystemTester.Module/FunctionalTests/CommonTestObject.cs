using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace XpandSystemTester.Module.FunctionalTests {
    [DefaultClassOptions]
    public class CommonTestObject:BaseObject {
        protected CommonTestObject(Session session) : base(session){
        }

        public string Name { get; set; }
    }
}
