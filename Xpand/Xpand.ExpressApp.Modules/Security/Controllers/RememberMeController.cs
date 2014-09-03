using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Controllers {
    public class RememberMeController : WindowController {
        public RememberMeController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Application.LoggingOff += ApplicationOnLoggingOff;
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Application.LoggingOff -= ApplicationOnLoggingOff;
        }

        protected virtual void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs){
            if (!loggingOffEventArgs.CanCancel)
                return;
            const string rememberMePropertyName = "RememberMe";

            var logonParameters = SecuritySystem.LogonParameters as IXpandLogonParameters;
            if (logonParameters != null) {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(logonParameters.GetType());
                var memberInfo = typeInfo.FindMember(rememberMePropertyName);
                if (memberInfo != null) memberInfo.SetValue(logonParameters, false);
                ObjectSerializer.WriteObjectPropertyValues(null, logonParameters.Storage, logonParameters);
                Application.WriteLastLogonParameters();
            }
        }
    }
}
