using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Reports.Win {
    [ToolboxBitmap(typeof(ReportsWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class ReportsWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule));
            return requiredModuleTypesCore;
        }
    }
}