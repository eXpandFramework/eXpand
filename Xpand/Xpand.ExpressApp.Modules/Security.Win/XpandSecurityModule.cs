using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.Security.Win {
    [ToolboxBitmap(typeof(XpandSecurityWinModule))]
    [ToolboxItem(true)]
    public sealed class XpandSecurityWinModule : XpandModuleBase {
        public XpandSecurityWinModule() {
            RequiredModuleTypes.Add(typeof(XpandSecurityModule));
        }
    }
}