using System;
using System.ComponentModel;
using Xpand.ExpressApp.Security.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web.AuthenticationProviders {
    public class AnonymousAuthenticationStandard : XpandAuthenticationStandard {
        public AnonymousAuthenticationStandard(){
        }

        public AnonymousAuthenticationStandard(Type userType, Type logonParametersType) : base(userType, logonParametersType){
        }

        public override bool AskLogonParametersViaUI {
            get {
                var b = string.IsNullOrEmpty(LogonParameters.UserName) || LogonParameters.AnonymousUserName != LogonParameters.UserName;
                return b && !string.IsNullOrEmpty(LogonParameters.UserName) ? !LogonParameters.AnonymousLogin : b;
            }
        }

        [Browsable(false)]
        public new AnonymousLogonParameters LogonParameters {
            get { return (AnonymousLogonParameters) base.LogonParameters; }
        }

    }
}