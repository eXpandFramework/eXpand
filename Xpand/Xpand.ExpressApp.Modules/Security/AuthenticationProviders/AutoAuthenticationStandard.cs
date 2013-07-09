using System;
using DevExpress.ExpressApp.Security;
using DevExpress.Utils;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    [ToolboxTabName(XpandAssemblyInfo.TabSecurity)]
    public class AutoAuthenticationStandard : AuthenticationStandard {
        public AutoAuthenticationStandard() { }
        public AutoAuthenticationStandard(Type userType, Type logonParametersType) : base(userType, logonParametersType) { }

        public override bool AskLogonParametersViaUI {
            get { return false; }
        }
    }
}
