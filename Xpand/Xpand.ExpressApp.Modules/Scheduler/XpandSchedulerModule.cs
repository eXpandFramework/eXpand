using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Scheduler {
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinWebModules)]
    public sealed class XpandSchedulerModule : XpandModuleBase,IDashboardUser {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Scheduler.SchedulerModuleBase));
            return requiredModuleTypesCore;
        }
    }
}