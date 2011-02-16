using System;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;

namespace Xpand.ExpressApp.Security.Controllers {
    public class RememberMeController : WindowController {
        

        public RememberMeController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            Application.LoggingOff += ApplicationOnLoggingOff;
        }


        void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            if (HttpContext.Current != null) {
                var httpCookie = HttpContext.Current.Response.Cookies[Application.ApplicationName + "RememberMe"];
                if (httpCookie != null) httpCookie.Expires = DateTime.Now.AddDays(-1);
            } else {
                var typeInfo = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.LogonParameters.GetType());
                var memberInfo = typeInfo.FindMember("RememberMe");
                if (memberInfo != null) memberInfo.SetValue(SecuritySystem.LogonParameters, false);
                var logonParameterStoreCore = ((ISupportLogonParameterStore) sender).CreateLogonParameterStoreCore();
                ObjectSerializer.WriteObjectPropertyValues(null, logonParameterStoreCore, SecuritySystem.LogonParameters);
            }

        }
    }

}
