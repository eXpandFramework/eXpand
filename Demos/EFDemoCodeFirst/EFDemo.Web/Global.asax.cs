using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Reflection;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.AuditTrail;
using DevExpress.Web.ASPxClasses;
using EFDemo.Module;
using EFDemo.Module.Data;

namespace EFDemo.Web {
    public class Global : System.Web.HttpApplication {
#if CodeFirst
		private static Assembly CurrentDomain_AssemblyResolve(Object sender, ResolveEventArgs args) {
			if(args.Name.Contains(Consts.EntityFrameworkAssemblyName + ",")) {
				String text = String.Format(@"Could not load assembly ""{0}"".", args.Name) + "\r\n\r\n" + Consts.CodeFirstMessageText + "\r\n";
				throw new Exception(text);
			}
			return null;
		}
#endif
		public Global() {
            InitializeComponent();
        }
        protected void Application_Start(Object sender, EventArgs e) {
#if CodeFirst
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
#endif

			RenderHelper.RenderMode = DevExpress.Web.ASPxClasses.ControlRenderMode.Lightweight;
            ASPxWebControl.CallbackError += new EventHandler(Application_Error);
#if DEBUG
			TestScriptsManager.EasyTestEnabled = true;
#endif
		}
        protected void Session_Start(Object sender, EventArgs e) {
            WebApplication.SetInstance(Session, new EFDemoWebApplication());
            AuditTrailService.Instance.CustomizeAuditTrailSettings += new CustomizeAuditSettingsEventHandler(Instance_CustomizeAuditTrailSettings);
            AuditTrailService.Instance.QueryCurrentUserName += new QueryCurrentUserNameEventHandler(Instance_QueryCurrentUserName);
            WebApplication.Instance.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(Instance_LastLogonParametersReading);
            WebApplication.Instance.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(Instance_CustomizeFormattingCulture);
            if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;

            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
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
