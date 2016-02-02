using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Loggers;
using DevExpress.ExpressApp.EasyTest.WebAdapter;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;
using HideScrollBarCommand = Xpand.ExpressApp.EasyTest.WebAdapter.Commands.HideScrollBarCommand;
using MethodInvoker = System.Windows.Forms.MethodInvoker;
using SetWebMaxWaitTimeOutCommand = Xpand.ExpressApp.EasyTest.WebAdapter.Commands.SetWebMaxWaitTimeOutCommand;

[assembly: Adapter(typeof (XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter{
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter, IXpandTestAdapter{
        private XpandWebCommandAdapter _webCommandAdapter;

        public override void RunApplication(TestApplication testApplication){
            testApplication.Assign();
            testApplication.CreateParametersFile();
            testApplication.DeleteUserModel();
            testApplication.CopyModel();
            ConfigApplicationModel(testApplication);
            base.RunApplication(testApplication);
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
            FileDownloadDialogHelper.SaveDialogOpened = false;
            _webCommandAdapter = new XpandWebCommandAdapter(this);
            _webCommandAdapter.WaitForBrowserResponse(false);
            Win32Helper.MoveMousePointTo(new Point(0, 0));
            return _webCommandAdapter;
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            testApplication.ClearModel();
            testApplication.DeleteParametersFile();
            ScreenCaptureCommand.Stop();
            base.KillApplication(testApplication, context);
        }

        public override void RegisterCommands(IRegisterCommand registrator){
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(Xpand.EasyTest.Commands.HideScrollBarCommand.Name, typeof (HideScrollBarCommand));
            registrator.RegisterCommand(Xpand.EasyTest.Commands.SetWebMaxWaitTimeOutCommand.Name, typeof(SetWebMaxWaitTimeOutCommand));
        }

    }


    public class StandaloneWebBrowserCollection : DevExpress.ExpressApp.EasyTest.WebAdapter.StandaloneWebBrowserCollection,IWebBrowserCollection {
        public const string EasyTestBrowser = "EasyTest Browser";
        IEasyTestWebBrowser IWebBrowserCollection.CreateWebBrowser() {
            var webBrowser = CreateWebBrowser();
            var standaloneWebBrowser = ((XAFStandaloneWebBrowser)webBrowser);
            var browserControl = ((StandaloneWebBrowserControl) standaloneWebBrowser.WebBrowser);
            browserControl.ScrollBarsEnabled = false;
            var form = browserControl.Parent;
            form.Invoke(new MethodInvoker(delegate{
                form.Location = new Point(0, 0);
                form.Text = EasyTestBrowser;
                if (WebBrowserCollection.DefaultFormSize!=new Size()){
                    form.Width = WebBrowserCollection.DefaultFormSize.Width;
                    form.Height = WebBrowserCollection.DefaultFormSize.Height;
                }
            }));
            return webBrowser;
        }
    }

    public class XpandWebCommandAdapter : WebCommandAdapter{
        private readonly IApplicationAdapter _adapter;

        public XpandWebCommandAdapter(DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter adapter) : base(adapter){
            _adapter = adapter;
        }

        public IApplicationAdapter Adapter {
            get { return _adapter; }
        }
        
    }

}
