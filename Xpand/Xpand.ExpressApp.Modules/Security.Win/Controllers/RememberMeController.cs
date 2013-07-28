using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.Security.Win.Controllers {
    public class RememberMeController:Security.Controllers.RememberMeController {
        protected override void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            if (!loggingOffEventArgs.CanCancel)
                return;
            const string rememberMePropertyName = "RememberMe";

            var logonParameters = SecuritySystem.LogonParameters as IXpandLogonParameters;
            if (logonParameters!=null) {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(logonParameters.GetType());
                var memberInfo = typeInfo.FindMember(rememberMePropertyName);
                if (memberInfo != null) memberInfo.SetValue(logonParameters, false);
                ObjectSerializer.WriteObjectPropertyValues(null, logonParameters.Storage, logonParameters);
            }
        }
    }
}
