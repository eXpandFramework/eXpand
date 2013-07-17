using System;
using System.Web;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Security.Web {
    public class RememberMeController : Xpand.ExpressApp.Security.Controllers.RememberMeController {
        protected override void ApplicationOnLoggingOff(object sender, LoggingOffEventArgs loggingOffEventArgs) {
            return;
            if (!loggingOffEventArgs.CanCancel)
                return;
            const string rememberMePropertyName = "RememberMe";

            var httpCookie = HttpContext.Current.Response.Cookies[Application.ApplicationName + rememberMePropertyName];
            if (httpCookie != null) httpCookie.Expires = DateTime.Now.AddDays(-1);

        }
    }

}
