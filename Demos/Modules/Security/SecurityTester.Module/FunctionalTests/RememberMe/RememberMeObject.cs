using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SecurityTester.Module.FunctionalTests.RememberMe {
    [DefaultClassOptions]
    public class RememberMeObject:BaseObject {
        public RememberMeObject(Session session) : base(session){
        }
    }
}
