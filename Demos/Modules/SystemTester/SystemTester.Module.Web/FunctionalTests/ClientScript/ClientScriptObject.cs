using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SystemTester.Module.Web.FunctionalTests.ClientScript {
    [DefaultClassOptions]
    public class ClientScriptObject:BaseObject {
        public ClientScriptObject(Session session) : base(session){
        }
    }
}
