using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using Fasterflect;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    class XpandWebBrowserCollection : WebBrowserCollection, IWebBrowserCollection{
        IEasyTestWebBrowser IWebBrowserCollection.CreateWebBrowser(){
            var xafWebBrowser = new XAFWebBrowser(this);
            Add(xafWebBrowser);
            return xafWebBrowser;
        }

        void IWebBrowserCollection.WaitForAllWebBrowsersResponse(){
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WaitForAllWebBrowsersResponse();
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds>5000)
                WaitForAllWebBrowsersResponse();
        }
    }
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter{
        private Process _process;
        private const string SingleWebDevParamName = "SingleWebDev";
        private const string UrlParamName = "Url";
        public override void RunApplication(TestApplication testApplication){
            if (!GetParamValue("UseIISExpress", false, testApplication)){
                RunApplicationBase(testApplication);
            }
            else{
                string url = testApplication.GetParamValue(UrlParamName);
                var uri = new Uri(url);
                webBrowsers = CreateWebBrowsers(testApplication);
                
                if (!WebDevWebServerHelper.IsWebDevServerStarted(uri)){
                    _process = IISExpressServerHelper.Run(testApplication,uri);
                }
                if (testApplication.FindParamValue("DefaultWindowSize") != null) {
                    WebBrowserCollection.DefaultFormSize = GetWindowSize(testApplication.GetParamValue("DefaultWindowSize"));
                }
                this.CallMethod("CreateBrowser", url);
            }
        }

        public override ICommandAdapter CreateCommandAdapter(){
            FileDownloadDialogHelper.SaveDialogOpened = false;
            var webCommandAdapter = new XpandWebCommandAdapter(this);
            webCommandAdapter.WaitForBrowserResponse(false);
            Win32Helper.MoveMousePointTo(new Point(0, 0));
            return webCommandAdapter;
        }

        private void RunApplicationBase(TestApplication testApplication){
            string url = testApplication.GetParamValue(UrlParamName);
            var uri = new Uri(url);
            
            webBrowsers=CreateWebBrowsers(testApplication);
            string physicalPath = testApplication.FindParamValue("PhysicalPath");
            if (string.IsNullOrEmpty(physicalPath) && !GetParamValue("DontRestartIIS", false, testApplication)) {
                RestartIIS();
            }
            else {
                if (!GetParamValue("DontRunWebDev", false, testApplication) && !string.IsNullOrEmpty(physicalPath)) {
                    typeof(DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter).CallMethod("LoadFileInfo",Path.GetFullPath(physicalPath));
                    if (testApplication.FindParamValue(SingleWebDevParamName) == null) {
                        if (WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
                            WebDevWebServerHelper.KillWebDevWebServer();
                        }
                        WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString(CultureInfo.InvariantCulture));
                    }
                    else {
                        if (!WebDevWebServerHelper.IsWebDevServerStarted(uri)) {
                            WebDevWebServerHelper.RunWebDevWebServer(Path.GetFullPath(physicalPath), uri.Port.ToString(CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
            if (testApplication.FindParamValue("DefaultWindowSize") != null) {
                WebBrowserCollection.DefaultFormSize = GetWindowSize(testApplication.GetParamValue("DefaultWindowSize"));
            }
            string waitDebuggerAttached = testApplication.FindParamValue("WaitDebuggerAttached");
            if (!string.IsNullOrEmpty(waitDebuggerAttached)) {
                Thread.Sleep(8000);
                if (Debugger.IsAttached) {
                    MessageBox.Show("Start web application?", "Warning", MessageBoxButtons.OK);
                }
            }
            DateTime current = DateTime.Now;
            while (!WebDevWebServerHelper.IsWebDevServerStarted(uri) && DateTime.Now.Subtract(current).TotalSeconds < 60) {
                Thread.Sleep(200);
            }
            this.CallMethod("CreateBrowser",url);
        }

        private IWebBrowserCollection CreateWebBrowsers(TestApplication testApplication){
            string webBrowserType = testApplication.FindParamValue("WebBrowserType");
            if (string.IsNullOrEmpty(webBrowserType)) {
                webBrowserType = "Default";
            }
            return webBrowserType == "Default"
                ? (IWebBrowserCollection) new XpandWebBrowserCollection()
                : new StandaloneWebBrowserCollection();
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            webBrowsers.KillAllWebBrowsers();
            bool isSingleWebDev = testApplication.FindParamValue(SingleWebDevParamName) != null;
            if (testApplication.FindParamValue("DontKillWebDev") == null&&_process!=null) {
                if (isSingleWebDev) {
                    if (context != KillApplicationConext.TestNormalEnded) {
                        IISExpressServerHelper.Stop(_process);
                    }
                }
                else {
                    IISExpressServerHelper.Stop(_process);
                }
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

    public class XpandWebCommandAdapter : WebCommandAdapter {
        public XpandWebCommandAdapter(DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter adapter) : base(adapter){
        }

    }

    public class IISExpressServerHelper{

        public static void Stop(Process process) {
            process.Kill();
        }

        public static Process Run(TestApplication testApplication, Uri uri) {
            string physicalPath = Path.GetFullPath(testApplication.FindParamValue("PhysicalPath"));
            string arguments = String.Format(@"/path:""{0}"" /port:{1}", physicalPath, uri.Port);
            EasyTestTracer.Tracer.InProcedure(String.Format("RunIISExpressServer({0})", arguments));
            try{
                string serverPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"IIS Express\iisexpress.exe");
                EasyTestTracer.Tracer.InProcedure(String.Format("IISExpressServerPath= {0}", serverPath));
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
                EasyTestTracer.Tracer.OutProcedure(String.Format("RunWebDevWebServer({0})", arguments));
            }
        }
    }
}