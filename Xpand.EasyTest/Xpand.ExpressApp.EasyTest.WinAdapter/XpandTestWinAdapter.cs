using System;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WinAdapter;

[assembly: Adapter(typeof(XpandTestWinAdapter))]

namespace Xpand.ExpressApp.EasyTest.WinAdapter {
    public class XpandTestWinAdapter : DevExpress.ExpressApp.EasyTest.WinAdapter.WinAdapter,IXpandTestWinAdapter {
        public override void RegisterCommands(IRegisterCommand registrator) {
            base.RegisterCommands(registrator);
            registrator.RegisterCommands(this);
            registrator.RegisterCommand(HideScrollBarCommand.Name, typeof (Commands.HideScrollBarCommand));
        }

        protected override WinEasyTestCommandAdapter InternalCreateCommandAdapter(int communicationPort, Type adapterType) {
            return base.InternalCreateCommandAdapter(communicationPort, typeof(XpandCustomEasyTestCommandAdapter));
        }
    }

    public class XpandCustomEasyTestCommandAdapter : WinEasyTestCommandAdapter {
        public XpandCustomEasyTestCommandAdapter() {
            TestControlFactoryWin.SetInstance(new XpandCustomTestControlFactory());
        }
    }

    public class XpandCustomTestControlFactory : TestControlFactoryWin {
//        public XpandCustomTestControlFactory() {
//            RegisterInterface<MyCustomTestControl>();
//        }
    }

}