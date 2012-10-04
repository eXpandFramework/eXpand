using System;
using System.Configuration;
using System.Web.Configuration;
using System.Web;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using SecurityDemo.Module;
using DevExpress.ExpressApp.Web.Demos;
using DevExpress.Web.ASPxClasses;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security.ClientServer;
using Xpand.ExpressApp.Security.Core;

namespace SecurityDemo.Web {
    public class Global : System.Web.HttpApplication {
        //private static ApplicationServerStarter starter = null;
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            WebApplication.DefaultPage = "DefaultVertical.aspx";
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
        }
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new SecurityDemoAspNetApplication());
            WebApplication.Instance.CreateCustomLogonWindowObjectSpace += new EventHandler<CreateCustomLogonWindowObjectSpaceEventArgs>(Instance_CreateCustomLogonWindowObjectSpace);
            WebApplication.Instance.CreateCustomLogonWindowControllers += new EventHandler<CreateCustomLogonWindowControllersEventArgs>(Instance_CreateCustomLogonWindowControllers);


            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }

            SecurityStrategyComplex security = new SecurityStrategyComplex(typeof(SecurityDemoUser), typeof(XpandRole), new SecurityDemoAuthentication());
            WebApplication.Instance.Security = security;
            WebApplication.Instance.DatabaseVersionMismatch += delegate(object sender2, DatabaseVersionMismatchEventArgs e2) {
                e2.Updater.Update();
                e2.Handled = true;
            };

            //You do not need this call, the functionality is build in the XpandObjectSpaceProvider. To enable or disable use Model/Options/ClientSideSecurity


            //            WebApplication.Instance.CreateCustomObjectSpaceProvider += delegate(object sender3, CreateCustomObjectSpaceProviderEventArgs e3) {
            //                e3.ObjectSpaceProvider = new SecuredObjectSpaceProvider(security, e3.ConnectionString, e3.Connection);
            //            };

            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();

        }

        private void Instance_CreateCustomLogonWindowControllers(object sender, CreateCustomLogonWindowControllersEventArgs e) {
            e.Controllers.Add(((XafApplication)sender).CreateController<DevExpress.ExpressApp.Demos.ShowHintController>());
        }

        private void Instance_CreateCustomLogonWindowObjectSpace(object sender, CreateCustomLogonWindowObjectSpaceEventArgs e) {
            e.ObjectSpace = ((XafApplication)sender).CreateObjectSpace();
            ((SecurityDemoAuthenticationLogonParameters)e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        protected void Application_BeginRequest(Object sender, EventArgs e) {
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if (!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
                HttpContext.Current.Response.End();
            }
            // switching to the vertical page template
            if (HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("default.asp") || HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/")) {
                string pagePath = HttpContext.Current.Request.ApplicationPath;
                if (!pagePath.EndsWith("/")) {
                    pagePath += "/";
                }
                pagePath += "DefaultVertical.aspx";
                string redirectPath = pagePath + HttpContext.Current.Request.Url.Query;
                HttpContext.Current.Response.Redirect(redirectPath);
            }
        }
        protected void Application_EndRequest(Object sender, EventArgs e) {
        }
        protected void Application_AuthenticateRequest(Object sender, EventArgs e) {
        }
        protected void Application_Error(Object sender, EventArgs e) {
            ErrorHandling.Instance.ProcessApplicationError();
        }
        protected void Session_End(Object sender, EventArgs e) {
            WebApplication.DisposeInstance(Session);
        }
        protected void Application_End(Object sender, EventArgs e) {
        }
        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
        }
        #endregion
    }
}
