using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [ToolboxTabName(XpandAssemblyInfo.TabSecurity)]
    public class XpandAuthenticationStandard : AuthenticationStandard {
        public XpandAuthenticationStandard() {
        }

        public XpandAuthenticationStandard(Type userType, Type logonParametersType)
            : base(userType, logonParametersType) {
        }

        public override bool AskLogonParametersViaUI {
            get {
                var xpandLogonParameters = LogonParameters as XpandLogonParameters;
                return xpandLogonParameters == null || (!xpandLogonParameters.RememberMe || !(!(string.IsNullOrEmpty(
                    xpandLogonParameters.Password)) && !(string.IsNullOrEmpty(xpandLogonParameters.UserName))));
            }
        }

    }
}