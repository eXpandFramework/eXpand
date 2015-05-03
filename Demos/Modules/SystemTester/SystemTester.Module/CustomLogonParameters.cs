using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.Persistent.Base.Security;

namespace SystemTester.Module{
    [NonPersistent]
    public class CustomLogonParameters : AuthenticationStandardLogonParameters,IDBServerParameter {
        public string DBServer { get; set; }
    }
}