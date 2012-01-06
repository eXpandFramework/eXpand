using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.Security {
    [ToolboxBitmap(typeof(XpandSecurityModule))]
    [ToolboxItem(true)]
    public sealed partial class XpandSecurityModule : XpandModuleBase {
        public XpandSecurityModule() {
            InitializeComponent();
            AdditionalExportedTypes.AddRange(ModuleHelper.CollectExportedTypesFromAssembly(Assembly.GetAssembly(typeof(SecurityRole))));
        }
    }
}
