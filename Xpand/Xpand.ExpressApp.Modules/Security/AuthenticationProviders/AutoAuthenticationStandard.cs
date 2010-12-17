using System;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    public class AutoAuthenticationStandard : AuthenticationStandard {
        public AutoAuthenticationStandard() { }
        public AutoAuthenticationStandard(Type userType, Type logonParametersType) : base(userType, logonParametersType) { }

        public override bool AskLogonParametersViaUI {
            get { return true; }
        }
    }
}
