using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SecurityTester.Module.Web.FunctionalTests.Anonymous {
    [DefaultClassOptions]
    public class AnonymousObject:BaseObject {
        public AnonymousObject(Session session) : base(session){
        }
    }
}
