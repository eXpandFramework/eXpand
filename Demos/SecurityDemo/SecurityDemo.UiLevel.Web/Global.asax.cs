using System;
using System.Configuration;
using System.IO;
using System.Web;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Web.ASPxClasses;
using SecurityDemo.Module;
using SecurityDemo.UiLevel.Web.ApplicationCode;

namespace SecurityDemo.UiLevel.Web{
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
            WebApplication.SetInstance(Session, new SecurityDemoAspNetApplication());
            WebApplication.Instance.CreateCustomLogonWindowObjectSpace += Instance_CreateCustomLogonWindowObjectSpace;
            WebApplication.Instance.CreateCustomLogonWindowControllers += Instance_CreateCustomLogonWindowControllers;
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				WebApplication.Instance.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#else

            if (ConfigurationManager.AppSettings["SiteMode"] != null &&
                ConfigurationManager.AppSettings["SiteMode"].ToLower() == "true"){
                InMemoryDataStoreProvider.Register();
                WebApplication.Instance.ConnectionString = InMemoryDataStoreProvider.ConnectionString;
            }
            else{
                if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null){
                    WebApplication.Instance.ConnectionString =
                        ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
            }
#endif


            WebApplication.Instance.Setup();
            WebApplication.Instance.Start();
        }

        private void Instance_CreateCustomLogonWindowControllers(object sender,
            CreateCustomLogonWindowControllersEventArgs e){
            e.Controllers.Add(((XafApplication) sender).CreateController<ShowHintController>());
        }

        private void Instance_CreateCustomLogonWindowObjectSpace(object sender,
            CreateCustomLogonWindowObjectSpaceEventArgs e){
            e.ObjectSpace = ((XafApplication) sender).CreateObjectSpace();
            ((SecurityDemoAuthenticationLogonParameters) e.LogonParameters).ObjectSpace = e.ObjectSpace;
        }

        protected void Application_BeginRequest(Object sender, EventArgs e){
            string filePath = HttpContext.Current.Request.PhysicalPath;
            if (!string.IsNullOrEmpty(filePath)
                && (filePath.IndexOf("Images", StringComparison.Ordinal) >= 0) && !File.Exists(filePath)){
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