using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard.Win;

namespace Xpand.ExpressApp.Reports.Win {
    [ToolboxBitmap(typeof(XpandReportsWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class XpandReportsWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule));
            requiredModuleTypesCore.Add(typeof(DashboardWindowsFormsModule));
            return requiredModuleTypesCore;
        }
    }
}