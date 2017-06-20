using System;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web;
using Fasterflect;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Security.Web.Controllers {
    public class AnonymousLogonParamsController : ViewController<DetailView> {
        protected override void OnActivated(){
            base.OnActivated();
            var anonymousAuthentication = ((IModelOptionsAuthentication)Application.Model.Options).Athentication.AnonymousAuthentication;
            if (SecuritySystem.LogonParameters != null && (View.ObjectTypeInfo.Type==SecuritySystem.LogonParameters.GetType()&& anonymousAuthentication.Enabled)){
                var cancelAction = Frame.GetController<WebLogonController>().CancelAction;
                cancelAction.ActivateKey("Web application logon");
                cancelAction.Execute+=CancelActionOnExecute;
                if (string.Equals(View.CurrentObject.GetPropertyValue("UserName"),
                    anonymousAuthentication.AnonymousUser))
                    View.CurrentObject.SetPropertyValue("UserName", null);
            }
        }

        private void CancelActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs){
            ((SimpleAction) sender).Execute-=CancelActionOnExecute;
            Application.LogOff();
            AnonymousLogonController.InvalidateAnonymousCookie();
            
        }
    }

    public class AnonymousLogonController : Security.Controllers.AnonymousLogonController{
        public static readonly string CookieName = typeof(AnonymousLogonController).Name;

        protected override void OnActivated(){
            base.OnActivated();
            var authenticationEnabled = ((IModelOptionsAuthentication) Application.Model.Options).Athentication.AnonymousAuthentication.Enabled;
            Active[typeof(AnonymousLogonController).Name] = authenticationEnabled;
            if (authenticationEnabled){
                var logoffController = Frame.GetController<LogoffController>();
                if (logoffController != null) logoffController.LogoffAction.Execute += LogoffActionOnExecute;
            }
        }


        public static void InvalidateAnonymousCookie() {
            var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName);
            if (httpCookie != null) {
                HttpContext.Current.Request.Cookies.Remove(CookieName);
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.SetCookie(httpCookie);
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var logoffController = Frame.GetController<LogoffController>();
            if (logoffController != null) logoffController.LogoffAction.Execute -= LogoffActionOnExecute;
        }

        private void LogoffActionOnExecute(object sender, SimpleActionExecuteEventArgs e){
            if (((SimpleAction) sender).Caption== ((IModelOptionsAuthentication)Application.Model.Options).Athentication.AnonymousAuthentication.LoginActionCaption){
                var httpCookie = HttpContext.Current.Request.Cookies.Get(CookieName) ?? new HttpCookie(CookieName);
                httpCookie.Expires = DateTime.Now.AddSeconds(2);
                HttpContext.Current.Response.SetCookie(httpCookie);
            }
            else{
                InvalidateAnonymousCookie();
            }
        }

        

    }
}
