using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WinAdapter;

[assembly: Adapter(typeof(XpandTestWinAdapter))]

namespace Xpand.ExpressApp.EasyTest.WinAdapter {
    public class XpandTestWinAdapter : DevExpress.ExpressApp.EasyTest.WinAdapter.WinAdapter,IXpandTestWinAdapter {
        private WinEasyTestCommandAdapter _easyTestCommandAdapter;

        public override void RegisterCommands(IRegisterCommand registrator) {
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(HideScrollBarCommand.Name, typeof (Commands.HideScrollBarCommand));
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context){
            ScreenCaptureCommand.Stop();
            if (_easyTestCommandAdapter != null) {
                _easyTestCommandAdapter.Disconnect();
            }
            CloseApplication(mainProcess,true);
        }

        void CloseApplication(Process process, bool force) {
            int num = force ? 1 : 0x2710;
            DateTime now = DateTime.Now;
            TimeSpan zero = TimeSpan.Zero;
            bool flag = false;
            IntPtr ptr = IntPtr.Zero;
            while (zero.TotalMilliseconds < num){
                Trace.WriteLine(DateTime.Now + "\tCall Process.Refresh() at WinApplicationAdapter.CloseApplication");
                process.Refresh();
                Trace.WriteLine(DateTime.Now + "\tCall Process.Refresh() success");
                Trace.WriteLine(DateTime.Now +"\tCall Process.MainWindowHandle at WinApplicationAdapter.CloseApplication");
                IntPtr mainWindowHandle = IntPtr.Zero;
                try{
                    mainWindowHandle = process.MainWindowHandle;
                }
                catch (InvalidOperationException){
                }
                if ((mainWindowHandle != ptr) && (mainWindowHandle != IntPtr.Zero)){
                    ptr = mainWindowHandle;
                    try{
                        process.CloseMainWindow();
                    }
                    catch (Exception exception){
                        Logger.Instance.AddMessage(string.Format("Process{0}.CloseMainWindow return error:\n'{1}'",process.ProcessName, exception.Message));
                    }
                }
                Trace.WriteLine(DateTime.Now + "\tCall Process.MainWindowHandle success");
                try{
                    if (process.WaitForExit(Math.Min(0x3e8, num - ((int) zero.TotalMilliseconds)))){
                        flag = true;
                        break;
                    }
                }
                catch (Exception exception2){
                    Logger.Instance.AddMessage(string.Format("Process.WaitForExit return error:\n'{0}'",exception2.Message));
                }
                zero = DateTime.Now - now;
            }
            if (!flag){
                if (!force){
                    Logger.Instance.AddMessage(string.Format("The process '{0}' was not closed in '{1}' millisecond after the Process.CloseMainWindow was invoked, trying to kill this process",process.ProcessName, num));
                }
                try{
                    process.Kill();
                }
                catch (Exception exception3){
                    Logger.Instance.AddMessage(string.Format("Process name: '{0}' is not killed.\nReason:\n'{1}'",process.ProcessName, exception3.Message));
                }
                if (!process.WaitForExit(0x2710)){
                    throw new WarningException(
                        string.Format("Process name: '{0}' doesn't exited in 10 seconds after the Process.Kill was invoked",process.ProcessName));
                }
            }
        }

        protected override void InternalRun(string appName, string arguments){
            var directoryName = Path.GetDirectoryName(appName) + "";
            foreach (var file in Directory.GetFiles(directoryName, "Model.user*.xafml").ToArray()) {
                File.Delete(file);
            }
            var logonparameters = Path.Combine(directoryName, "logonparameters");
            if (File.Exists(logonparameters))
                File.Delete(logonparameters);
            base.InternalRun(appName, arguments);
        }

        protected override WinEasyTestCommandAdapter InternalCreateCommandAdapter(int communicationPort, Type adapterType){
            _easyTestCommandAdapter = base.InternalCreateCommandAdapter(communicationPort, typeof(XpandEasyTestCommandAdapter));
            return _easyTestCommandAdapter;
        }
    }

    public class XpandEasyTestCommandAdapter : WinEasyTestCommandAdapter {
        public XpandEasyTestCommandAdapter() {
            TestControlFactoryWin.SetInstance(new XpandCustomTestControlFactory());
        }

    }

    public class XpandCustomTestControlFactory : TestControlFactoryWin {
//        public XpandCustomTestControlFactory() {
//            RegisterInterface<MyCustomTestControl>();
//        }
    }

}