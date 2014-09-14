using System;
using System.IO;
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
            base.KillApplication(testApplication, context);
        }

        public override void RunApplication(TestApplication testApplication){
            base.RunApplication(testApplication);
            var directoryName = Path.GetDirectoryName(testApplication.GetParamValue("FileName"))+"";
            var path = Path.Combine(directoryName,"Model.User.xafml");
            if (File.Exists(path))
                File.Delete(path);
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