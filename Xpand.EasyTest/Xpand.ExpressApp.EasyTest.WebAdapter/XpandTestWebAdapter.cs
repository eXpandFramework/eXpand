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
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;
using Xpand.Utils.Automation;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter, IXpandTestAdapter{
        private XpandWebCommandAdapter _webCommandAdapter;
        private IWebServerManager _serverManager;

        public override void RunApplication(TestApplication testApplication){
            testApplication.Assign();
            testApplication.CreateParametersFile();
            testApplication.DeleteUserModel();
            testApplication.CopyModel();
            ConfigApplicationModel(testApplication);
            if (testApplication.ParameterValue<bool>(ApplicationParams.UseIIS)) {
                IISHelper.Configure(testApplication);
            }  
            base.RunApplication(testApplication);
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
            _webCommandAdapter = new XpandWebCommandAdapter(WebBrowser);
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
            if (testApplication.ParameterValue<bool>(ApplicationParams.UseIIS))
                IISHelper.StopAplicationPool(testApplication);
        }

        private void KillApplicationBase(KillApplicationConext context){
            this.CallMethod("CloseWebBrowser");
            _serverManager.ProcessKillApplication(context);
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

        public XpandWebCommandAdapter(IEasyTestWebBrowser webBrowser) : base(webBrowser){
        }

        public IntPtr MainWindowHandle => WebBrowser.BrowserWindowHandle;

        
    }

}
