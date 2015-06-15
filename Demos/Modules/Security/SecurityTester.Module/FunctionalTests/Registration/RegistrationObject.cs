using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace SecurityTester.Module.FunctionalTests.Registration {
    [DefaultClassOptions]
    public class RegistrationObject:BaseObject {
        public RegistrationObject(Session session) : base(session){
        }

        public string UserName {
            get {
                return SecuritySystem.CurrentUserName;
            }
        }
    }
}
