using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    public class XpandAuthenticationStandard : AuthenticationStandard {
        private readonly XpandLogonParameters parameters = new XpandLogonParameters();

        public override bool AskLogonParametersViaUI {
            get { return parameters.RememberMe ? !(!(string.IsNullOrEmpty(parameters.Password)) && !(string.IsNullOrEmpty(parameters.UserName))) : true; }
        }

        public override object LogonParameters {
            get {
                return parameters;
            }
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            if (string.IsNullOrEmpty(parameters.UserName))
                throw new ArgumentException(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.UserNameIsEmpty));
            var user = (IAuthenticationStandardUser)objectSpace.FindObject(UserType, CriteriaOperator.Parse("UserName = ?", parameters.UserName));

            if (user == null || !user.ComparePassword(parameters.Password)) {
                throw new AuthenticationException(parameters.UserName, SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
            }
            return user;
        }
    }
}