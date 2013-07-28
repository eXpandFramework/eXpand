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
                return (!((XpandLogonParameters)LogonParameters).RememberMe || !(!(string.IsNullOrEmpty(
                    ((XpandLogonParameters)LogonParameters).Password)) && !(string.IsNullOrEmpty(
                  ((XpandLogonParameters)LogonParameters).UserName))));
            }
        }

    }
}