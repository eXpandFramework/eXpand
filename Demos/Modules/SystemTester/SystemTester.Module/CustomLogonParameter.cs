using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Controllers;

namespace SystemTester.Module {
    [NonPersistent]
    [Serializable]
    public class CustomLogonParameter : AuthenticationStandardLogonParameters,IDBServerParameter {
        public string DBServer { get; set; }
    }
}
