using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;

namespace Xpand.ExpressApp.Reports.Win {
    [ToolboxBitmap(typeof(XpandReportsWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandReportsWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Reports.Win.ReportsWindowsFormsModule));
            return requiredModuleTypesCore;
        }
    }
}