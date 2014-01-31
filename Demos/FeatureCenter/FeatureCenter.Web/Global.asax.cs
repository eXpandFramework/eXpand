using System;
using System.Configuration;
using System.IO;
using System.Web;
using DevExpress.ExpressApp.Web;
using DevExpress.Web.ASPxClasses;
using FeatureCenter.Web.ApplicationCode;

namespace FeatureCenter.Web{
    public class Global : HttpApplication{
        public Global(){
            InitializeComponent();
        }

        protected void Application_Start(Object sender, EventArgs e){
            ASPxWebControl.CallbackError += Application_Error;

#if EASYTEST
			DevExpress.ExpressApp.Web.TestScripts.TestScriptsManager.EasyTestEnabled = true;
#endif
        }

        protected void Session_Start(Object sender, EventArgs e){
            WebApplication.SetInstance(Session, new FeatureCenterAspNetApplication());
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null){
                WebApplication.Instance.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }

        protected void Application_BeginRequest(Object sender, EventArgs e){
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if (!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images") >= 0) && !File.Exists(filePath)){
                HttpContext.Current.Response.End();
            }
        }

        protected void Application_EndRequest(Object sender, EventArgs e){
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e){
        }

        protected void Application_Error(Object sender, EventArgs e){
            ErrorHandling.Instance.ProcessApplicationError();
        }

        protected void Session_End(Object sender, EventArgs e){
            WebApplication.LogOff(Session);
            WebApplication.DisposeInstance(Session);
        }

        protected void Application_End(Object sender, EventArgs e){
        }

        #region Web Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(){
        }

        #endregion
    }
}