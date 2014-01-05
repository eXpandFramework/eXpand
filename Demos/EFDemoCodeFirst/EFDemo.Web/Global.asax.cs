using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Web;
using DevExpress.DemoData.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Web.ASPxClasses;
using EFDemo.Module;
using EFDemo.Module.Data;

namespace EFDemo.Web {
    public class Global : System.Web.HttpApplication {
        public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if DEBUG
            TestScriptsManager.EasyTestEnabled = true;
#endif
        }
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new EFDemoWebApplication());
            AuditTrailService.Instance.CustomizeAuditTrailSettings += new CustomizeAuditSettingsEventHandler(Instance_CustomizeAuditTrailSettings);
            AuditTrailService.Instance.QueryCurrentUserName += new QueryCurrentUserNameEventHandler(Instance_QueryCurrentUserName);
            WebApplication webApplication = WebApplication.Instance;
            webApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(Instance_LastLogonParametersReading);
            webApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(Instance_CustomizeFormattingCulture);
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if(connectionStringSettings != null) {
                webApplication.ConnectionString = connectionStringSettings.ConnectionString;
            }
            else if(string.IsNullOrEmpty(webApplication.ConnectionString) && webApplication.Connection == null) {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                if(connectionStringSettings != null) {
                    webApplication.ConnectionString = DbEngineDetector.PatchConnectionString(connectionStringSettings.ConnectionString);
                }
            }
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;

            webApplication.Setup();
            webApplication.Start();
        }

        private void Instance_CustomizeFormattingCulture(Object sender, CustomizeFormattingCultureEventArgs e) {
            e.FormattingCulture = CultureInfo.GetCultureInfo("en-US");
        }
        private void Instance_LastLogonParametersReading(Object sender, LastLogonParametersReadingEventArgs e) {
            if(String.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }
        private void Instance_QueryCurrentUserName(Object sender, QueryCurrentUserNameEventArgs e) {
            e.CurrentUserName = String.Format("Web user ({0})", HttpContext.Current.Request.UserHostAddress);
        }
        private void Instance_CustomizeAuditTrailSettings(Object sender, CustomizeAuditTrailSettingsEventArgs e) {
            e.AuditTrailSettings.Clear();
            e.AuditTrailSettings.AddType(typeof(Contact), true);
        }
        protected void Application_BeginRequest(Object sender, EventArgs e) {
            String filePath = HttpContext.Current.Request.PhysicalPath;
            if(!String.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !System.IO.File.Exists(filePath)) {
                HttpContext.Current.Response.End();
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
