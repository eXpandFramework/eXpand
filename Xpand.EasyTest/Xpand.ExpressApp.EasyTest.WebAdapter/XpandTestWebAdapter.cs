using System;
using System.Diagnostics;
using System.IO;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using Fasterflect;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter{
        private const string UrlParamName = "Url";
        public override void RunApplication(TestApplication testApplication){
            if (!GetParamValue("UseIISExpress", false, testApplication)){
                base.RunApplication(testApplication);
            }
            else{
                string url = testApplication.GetParamValue(UrlParamName);
                var uri = new Uri(url);
                string webBrowserType = testApplication.FindParamValue("WebBrowserType");
                webBrowsers = string.IsNullOrEmpty(webBrowserType) ? (IWebBrowserCollection) new WebBrowserCollection() : new StandaloneWebBrowserCollection();
                
                if (!WebDevWebServerHelper.IsWebDevServerStarted(uri))
                    IISExpressServerHelper.Run(testApplication,uri);
                if (testApplication.FindParamValue("DefaultWindowSize") != null) {
                    WebBrowserCollection.DefaultFormSize = GetWindowSize(testApplication.GetParamValue("DefaultWindowSize"));
                }
                this.CallMethod("CreateBrowser", url);
            }
        }

        private bool GetParamValue(string name, bool defaultValue, TestApplication testApplication){
            string paramValue = testApplication.FindParamValue(name);
            bool result;
            if (string.IsNullOrEmpty(paramValue) || !bool.TryParse(paramValue, out result)){
                result = defaultValue;
            }
            return result;
        }

        public override void RegisterCommands(IRegisterCommand registrator){
            base.RegisterCommands(registrator);
            registrator.RegisterCommands();
        }
    }

    public class IISExpressServerHelper{
        public static Process Run(TestApplication testApplication, Uri uri) {
            string physicalPath = Path.GetFullPath(testApplication.FindParamValue("PhysicalPath"));
            string arguments = string.Format(@"/path:""{0}"" /port:{1}", physicalPath, uri.Port);
            EasyTestTracer.Tracer.InProcedure(string.Format("RunIISExpressServer({0})", arguments));
            try{
                string serverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS Express\iisexpress.exe");
                EasyTestTracer.Tracer.InProcedure(string.Format("IISExpressServerPath= {0}", serverPath));
                var serverProcess = new Process{
                    StartInfo ={
                        FileName = serverPath,
                        Arguments = arguments,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                serverProcess.Start();
                return serverProcess;
            }
            finally{
                EasyTestTracer.Tracer.OutProcedure(string.Format("RunWebDevWebServer({0})", arguments));
            }
        }

    }
}