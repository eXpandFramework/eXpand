using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using Xpand.EasyTest;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WinAdapter;

[assembly: Adapter(typeof(XpandTestWinAdapter))]

namespace Xpand.ExpressApp.EasyTest.WinAdapter {
    public class XpandTestWinAdapter : DevExpress.ExpressApp.EasyTest.WinAdapter.WinAdapter, IXpandTestWinAdapter {
        private WinEasyTestCommandAdapter _easyTestCommandAdapter;
        private static List<Process> _additionalProcesses;

        public override void RegisterCommands(IRegisterCommand registrator) {
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(HideScrollBarCommand.Name, typeof(Commands.HideScrollBarCommand));
        }

        void CloseApplication(IEnumerable<Process> appProcesses, bool force) {
            int totalTimeout = force ? 1 : 10000;
            foreach (Process appProcess in appProcesses) {
                var processName = appProcess.ProcessName;
                DateTime start = DateTime.Now;
                TimeSpan spendTime = TimeSpan.Zero;
                bool closed = false;
                IntPtr lastWindowForClose = IntPtr.Zero;
                while (spendTime.TotalMilliseconds < totalTimeout) {
                    Trace.WriteLine(DateTime.Now + "\tCall Process.Refresh() at WinApplicationAdapter.CloseApplication");
                    appProcess.Refresh();
                    Trace.WriteLine(DateTime.Now + "\tCall Process.Refresh() success");
                    Trace.WriteLine(DateTime.Now + "\tCall Process.MainWindowHandle at WinApplicationAdapter.CloseApplication");
                    IntPtr processWindowHandle = IntPtr.Zero;
                    try {
                        processWindowHandle = appProcess.MainWindowHandle;
                    }
                    catch (InvalidOperationException) { }
                    if (processWindowHandle != lastWindowForClose && processWindowHandle != IntPtr.Zero) {
                        lastWindowForClose = processWindowHandle;
                        try {
                            appProcess.CloseMainWindow();
                        }
                        catch (Exception e) {
                            Logger.Instance.AddMessage(string.Format("Process.CloseMainWindow return error:\n'{0}'", e.Message));
                        }
                    }
                    Trace.WriteLine(DateTime.Now + "\tCall Process.MainWindowHandle success");
                    try {
                        if (appProcess.WaitForExit(Math.Min(1000, (int)(totalTimeout - spendTime.TotalMilliseconds)))) {
                            closed = true;
                            break;
                        }
                    }
                    catch (Exception e) {
                        Logger.Instance.AddMessage(string.Format("Process.WaitForExit return error:\n'{0}'", e.Message));
                    }
                    spendTime = DateTime.Now - start;
                }
                if (!closed) {
                    if (!force) {
                        Logger.Instance.AddMessage(string.Format(
                            "The process '{0}' was not closed in '{1}' millisecond after " +
                            "the Process.CloseMainWindow was invoked, trying to kill this process",
                            processName, totalTimeout));
                    }
                    try {
                        appProcess.Kill();
                    }
                    catch (Exception e) {
                        Logger.Instance.AddMessage(string.Format("Process name: '{0}' is not killed.\nReason:\n'{1}'", processName, e.Message));
                    }
                    if (!appProcess.WaitForExit(10000)) {
                        throw new WarningException(string.Format("Process name: '{0}' doesn't exited in 10 seconds after the Process.Kill was invoked", processName));
                    }
                }
            }
        }

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context) {
            ScreenCaptureCommand.Stop();
            if (_easyTestCommandAdapter != null) {
                _easyTestCommandAdapter.Disconnect();
            }
            CloseApplication(new[]{mainProcess}, true);
            CloseApplication(_additionalProcesses.Where(process => !process.HasExited).ToArray(), true);
        }

        public override void RunApplication(TestApplication testApplication) {
            string appName = testApplication.GetParamValue("FileName");
            var directoryName = DeleteUserModel(appName);
            DeleteLogonParametersFile(directoryName);
            RunAdditionalApps(testApplication);
            base.RunApplication(testApplication);
        }

        private void RunAdditionalApps(TestApplication testApplication) {
            _additionalProcesses = new List<Process>();
            var additionalApps = testApplication.ParameterValue<string>("AdditionalApplications");
            if (!string.IsNullOrEmpty(additionalApps))
                foreach (var app in additionalApps.Split(';')) {
                    var fullPath = Path.GetFullPath(app);
                    var process = new Process { StartInfo = new ProcessStartInfo(fullPath) };
                    process.Start();
                    Thread.Sleep(15000);
                    _additionalProcesses.Add(process);
                }

        }

        private string DeleteUserModel(string appName) {
            var directoryName = Path.GetDirectoryName(appName) + "";
            foreach (var file in Directory.GetFiles(directoryName, "Model.user*.xafml").ToArray()) {
                File.Delete(file);
            }
            return directoryName;
        }

        private void DeleteLogonParametersFile(string directoryName) {
            var logonparameters = Path.Combine(directoryName, "logonparameters");
            if (File.Exists(logonparameters))
                File.Delete(logonparameters);
        }

        protected override WinEasyTestCommandAdapter InternalCreateCommandAdapter(int communicationPort, Type adapterType) {
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