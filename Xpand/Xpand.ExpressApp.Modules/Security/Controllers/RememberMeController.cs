using System;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.PersistentMetaData;

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

        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            if (!loggingOffEventArgs.CanCancel)
                return;
            const string rememberMePropertyName = "RememberMe";
            if (HttpContext.Current != null) {
                var httpCookie = HttpContext.Current.Response.Cookies[Application.ApplicationName + rememberMePropertyName];
                if (httpCookie != null) httpCookie.Expires = DateTime.Now.AddDays(-1);
            } else {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.LogonParameters.GetType());
                var memberInfo = typeInfo.FindMember(rememberMePropertyName);
                if (memberInfo != null) memberInfo.SetValue(SecuritySystem.LogonParameters, false);
                var logonParameterStoreCore = ((ISettingsStorage)sender).CreateLogonParameterStoreCore();
                ObjectSerializer.WriteObjectPropertyValues(null, logonParameterStoreCore, SecuritySystem.LogonParameters);
            }

        }
    }

}
