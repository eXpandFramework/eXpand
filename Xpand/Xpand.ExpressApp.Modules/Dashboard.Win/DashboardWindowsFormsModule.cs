using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Dashboard.Win {
    [ToolboxBitmap(typeof(DashboardWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class DashboardWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DashboardModule));
            return requiredModuleTypesCore;
        }
    }
}