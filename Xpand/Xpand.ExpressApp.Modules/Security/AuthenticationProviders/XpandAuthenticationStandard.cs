using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;

namespace Xpand.ExpressApp.Security.AuthenticationProviders {
    public class XpandAuthenticationStandard : AuthenticationStandard {
        public XpandAuthenticationStandard() {
        }

        private readonly XpandLogonParameters parameters = new XpandLogonParameters();

        public XpandAuthenticationStandard(Type userType, Type logonParametersType)
            : base(userType, logonParametersType) {
        }

        public override bool AskLogonParametersViaUI {
            get {
                return !((XpandLogonParameters)LogonParameters).RememberMe || !(!(string.IsNullOrEmpty(
                    ((XpandLogonParameters)LogonParameters).Password)) && !(string.IsNullOrEmpty(
                  ((XpandLogonParameters)LogonParameters).UserName)));
            }
        }

        public override object LogonParameters {
            get {
                return parameters;
            }
        }
        public override object Authenticate(IObjectSpace objectSpace) {
            if (string.IsNullOrEmpty(((XpandLogonParameters)LogonParameters).UserName))
                throw new ArgumentException(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.UserNameIsEmpty));
            var user = (IAuthenticationStandardUser)objectSpace.FindObject(UserType, CriteriaOperator.Parse("UserName = ?",
                                                                                                            ((XpandLogonParameters)
                                                                                                             LogonParameters)
                                                                                                                .UserName));

            if (user == null || !user.ComparePassword(((XpandLogonParameters)LogonParameters).Password)) {
                throw new AuthenticationException(((XpandLogonParameters)LogonParameters).UserName, SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
            }
            return user;
        }
    }
}