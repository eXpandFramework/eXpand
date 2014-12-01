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

        public override void KillApplication(TestApplication testApplication, KillApplicationConext context) {
            ScreenCaptureCommand.Stop();
            if (_easyTestCommandAdapter != null) {
                _easyTestCommandAdapter.Disconnect();
            }
            CloseApplication(mainProcess.ProcessName, true);
            foreach (var additionalProcess in _additionalProcesses) {
                CloseApplication(additionalProcess.ProcessName, true);
            }
        }

        public override void RunApplication(TestApplication testApplication) {
            string appName = testApplication.GetParamValue("FileName");
            var directoryName = DeleteUserModel(appName);
            DeleteLogonParametersFile(directoryName);
            RunAdditionalApps(testApplication);
            base.RunApplication(testApplication);
        }

        private static void RunAdditionalApps(TestApplication testApplication) {
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