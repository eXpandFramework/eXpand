using System.ComponentModel;
using System.Drawing;

namespace Xpand.ExpressApp.Dashboard.Win {
    [ToolboxBitmap(typeof(DashboardWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class DashboardWindowsFormsModule : XpandModuleBase {
        public DashboardWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
        }
    }
}