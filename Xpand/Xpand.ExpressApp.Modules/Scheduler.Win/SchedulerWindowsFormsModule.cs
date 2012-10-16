using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Scheduler.Win {
    [ToolboxBitmap(typeof(SchedulerWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class SchedulerWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            return requiredModuleTypesCore;
        }
    }
}