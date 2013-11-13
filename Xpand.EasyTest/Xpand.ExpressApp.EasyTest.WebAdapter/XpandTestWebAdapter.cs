using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.Commands;
using Xpand.ExpressApp.EasyTest.WebAdapter;

[assembly: Adapter(typeof(XpandTestWebAdapter))]

namespace Xpand.ExpressApp.EasyTest.WebAdapter {
    public class XpandTestWebAdapter : DevExpress.ExpressApp.EasyTest.WebAdapter.WebAdapter {
        public override void RegisterCommands(IRegisterCommand registrator) {
            base.RegisterCommands(registrator);
            registrator.RegisterCommands();
        }
    }
}