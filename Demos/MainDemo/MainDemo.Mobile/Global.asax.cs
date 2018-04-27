using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
//using DXHttpProxyClient;

namespace MainDemo.Mobile {
    public class Global : System.Web.HttpApplication {
        protected void Application_BeginRequest(object sender, EventArgs e) {
            CorsSupport.HandlePreflightRequest(HttpContext.Current);
        }
        protected void Application_Start(Object sender, EventArgs e) {
            DevExpress.XtraReports.Web.WebDocumentViewer.DefaultWebDocumentViewerContainer.Register<DevExpress.XtraReports.Web.WebDocumentViewer.IWebDocumentViewerReportResolver, DevExpress.ExpressApp.ReportsV2.Mobile.XafReportsResolver<MainDemoMobileApplication>>();
        }
    }
}