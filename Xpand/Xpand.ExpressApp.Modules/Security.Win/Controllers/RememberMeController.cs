using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Win.Controllers {
    public class RememberMeController:Security.Controllers.RememberMeController {
        protected override void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            if (!loggingOffEventArgs.CanCancel)
                return;
            const string rememberMePropertyName = "RememberMe";
            
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.LogonParameters.GetType());
            var memberInfo = typeInfo.FindMember(rememberMePropertyName);
            if (memberInfo != null) memberInfo.SetValue(SecuritySystem.LogonParameters, false);
            var logonParameterStoreCore = ((ISettingsStorage)sender).CreateLogonParameterStoreCore();
            ObjectSerializer.WriteObjectPropertyValues(null, logonParameterStoreCore, SecuritySystem.LogonParameters);
            
        }
    }
}
