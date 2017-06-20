using System;
using Xpand.ExpressApp.Security.AuthenticationProviders;

namespace Xpand.ExpressApp.Security.Web.AuthenticationProviders {
    [Obsolete("not used",true)]
    public class AnonymousAuthenticationStandard : XpandAuthenticationStandard {
        public AnonymousAuthenticationStandard(){
        }

        public AnonymousAuthenticationStandard(Type userType, Type logonParametersType) : base(userType, logonParametersType){
        }



    }
}