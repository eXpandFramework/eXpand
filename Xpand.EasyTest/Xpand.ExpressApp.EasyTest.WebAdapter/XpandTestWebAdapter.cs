using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using Fasterflect;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter{
        private Process _process;
        private const string SingleWebDevParamName = "SingleWebDev";
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
                
                if (!WebDevWebServerHelper.IsWebDevServerStarted(uri)){
                    _process = IISExpressServerHelper.Run(testApplication,uri);
                }
                if (testApplication.FindParamValue("DefaultWindowSize") != null) {
                    WebBrowserCollection.DefaultFormSize = GetWindowSize(testApplication.GetParamValue("DefaultWindowSize"));
                }
                this.CallMethod("CreateBrowser", url);
            }
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            bool isSingleWebDev = testApplication.FindParamValue(SingleWebDevParamName) != null;
            if (testApplication.FindParamValue("DontKillWebDev") == null) {
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

    public class IISExpressServerHelper{
        internal class NativeMethods {
            // Methods
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetTopWindow(IntPtr hWnd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool PostMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        }

        static void SendStopMessageToProcess(int pid) {
            try {
                for (IntPtr ptr = NativeMethods.GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = NativeMethods.GetWindow(ptr, 2)) {
                    uint num;
                    NativeMethods.GetWindowThreadProcessId(ptr, out num);
                    if (pid == num) {
                        var hWnd = new HandleRef(null, ptr);
                        NativeMethods.PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                        return;
                    }
                }
            }
            catch (ArgumentException) {
            }
        }

        public static void Stop(Process process) {
            SendStopMessageToProcess(process.Id);
            process.Close();
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