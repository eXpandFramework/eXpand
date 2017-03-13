using System;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Loggers;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using DevExpress.ExpressApp.EasyTest.WebAdapter.WebServerManagers;
using Fasterflect;
using SHDocVw;
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;
using Xpand.Utils.Automation;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter, IXpandTestAdapter{
        private XpandWebCommandAdapter _webCommandAdapter;
        private IWebServerManager _serverManager;

        public override void RunApplication(TestApplication testApplication,string connectionString){
            XafWebBrowser.OperationMillisecondsTimeout = 60000*2;
            testApplication.Assign();
            testApplication.CreateParametersFile();
            testApplication.DeleteUserModel();
            testApplication.CopyModel();
            ConfigApplicationModel(testApplication);
            if (testApplication.ParameterValue<bool>(ApplicationParams.UseIIS)) {
                IISHelper.Configure(testApplication);
            }  
            base.RunApplication(testApplication,connectionString);

            WebBrowser.BrowserWindowHandle.MoveWindow(new Rectangle(0,0,1024,768));
        }


        private void ConfigApplicationModel(TestApplication testApplication){
            var physicalPath = testApplication.ParameterValue<string>(ApplicationParams.PhysicalPath);
            var useModel = testApplication.ParameterValue<bool>(ApplicationParams.UseModel);
            if (useModel){
                var logPath = Logger.Instance.GetLogger<FileLogger>().LogPath;
                var model = Directory.GetFiles(logPath, "*.xafml").Single();
                File.Copy(Path.Combine(physicalPath, "Model.xafml"), Path.Combine(physicalPath, "restore.xafml"), true);
                File.Copy(model, Path.Combine(physicalPath, "Model.xafml"), true);
            }
            else{
                if (File.Exists(Path.Combine(physicalPath, "restore.xafml"))){
                    File.Copy(Path.Combine(physicalPath, "restore.xafml"), Path.Combine(physicalPath, "Model.xafml"), true);
                    File.Delete(Path.Combine(physicalPath, "restore.xafml"));
                }
            }
        }


        public override ICommandAdapter CreateCommandAdapter(){
            _webCommandAdapter = new XpandWebCommandAdapter(WebBrowser,_serverManager);
            Win32Helper.MoveMousePointTo(new Point(0, 0));
            return _webCommandAdapter;
        }

        protected override IWebServerManager CreateWebServerManager(Uri url, TestApplication testApplication){
            _serverManager = base.CreateWebServerManager(url, testApplication);
            return _serverManager;
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            KillApplicationBase(context);
            testApplication.ClearModel();
            testApplication.DeleteParametersFile();
            ScreenCaptureCommand.Stop(false);
//            if (testApplication.ParameterValue<bool>(ApplicationParams.UseIIS))
                IISHelper.StopAplicationPool(testApplication);
        }

        private void KillApplicationBase(KillApplicationConext context){
            this.CallMethod("CloseWebBrowser");
            _serverManager?.ProcessKillApplication(context);
        }

        public override void RegisterCommands(IRegisterCommand registrator){
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(HideScrollBarCommand.Name, typeof (Commands.HideScrollBarCommand));
            registrator.RegisterCommand(SetWebMaxWaitTimeOutCommand.Name, typeof(Commands.SetWebMaxWaitTimeOutCommand));
            registrator.RegisterCommand(XpandCheckValidationResultCommand.Name, typeof(Commands.XpandCheckValidationResultCommand));
            registrator.RegisterCommand(LogonCommand.Name, typeof(Commands.LogonCommand));
        }

    }

    public class XpandWebCommandAdapter : WebCommandAdapter,IXpandEasyTestCommandAdapter{
        private readonly IEasyTestWebBrowser _webBrowser;

        public XpandWebCommandAdapter(IEasyTestWebBrowser webBrowser, IWebServerManager serverManager) : base(webBrowser,serverManager){
            _webBrowser = webBrowser;
        }

        public XpandEasyTestWebBrowser WebBrowser => new XpandEasyTestWebBrowser(_webBrowser);

        public IntPtr MainWindowHandle => _webBrowser.BrowserWindowHandle;

        
    }

    public class XpandEasyTestWebBrowser:IEasyTestWebBrowser{
        private readonly IEasyTestWebBrowser _easyTestWebBrowser;

        public XpandEasyTestWebBrowser(IEasyTestWebBrowser easyTestWebBrowser){
            _easyTestWebBrowser = easyTestWebBrowser;
        }

        public IWebBrowser2 Browser => (IWebBrowser2) _easyTestWebBrowser.GetPropertyValue("InternetExplorer");

        public void Navigate(string url){
            _easyTestWebBrowser.Navigate(url);
        }

        public void Refresh(){
            _easyTestWebBrowser.Refresh();
        }

        public void GoBack(){
            _easyTestWebBrowser.GoBack();
        }

        public void GoForward(){
            _easyTestWebBrowser.GoForward();
        }

        public void SetWindowSize(int width, int height){
            _easyTestWebBrowser.SetWindowSize(width, height);
        }

        public object ExecuteScript(string script){
            return _easyTestWebBrowser.ExecuteScript(script);
        }

        public void Close(){
            _easyTestWebBrowser.Close();
        }

        public IntPtr BrowserWindowHandle => _easyTestWebBrowser.BrowserWindowHandle;

        public IntPtr DialogWindowHandle => _easyTestWebBrowser.DialogWindowHandle;
    }
}
