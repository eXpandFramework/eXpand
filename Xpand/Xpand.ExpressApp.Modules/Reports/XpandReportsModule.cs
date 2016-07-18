using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Reports {
    [ToolboxBitmap(typeof(XpandReportsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandReportsModule : XpandModuleBase, IDashboardInteractionUser {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DashboardModule));
            return requiredModuleTypesCore;
        }
    }
}