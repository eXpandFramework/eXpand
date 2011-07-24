using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(XpandSecurityModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandSecurityModule : XpandModuleBase {
        public XpandSecurityModule() {
            InitializeComponent();
        }
    }
}
