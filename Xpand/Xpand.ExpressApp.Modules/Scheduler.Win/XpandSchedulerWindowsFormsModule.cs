using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;

namespace Xpand.ExpressApp.Scheduler.Win {
    [ToolboxBitmap(typeof(XpandSchedulerWindowsFormsModule))]
    [ToolboxItem(true)]
    public sealed class XpandSchedulerWindowsFormsModule : XpandModuleBase {
        protected override ModuleTypeList GetRequiredModuleTypesCore() {
            ModuleTypeList requiredModuleTypesCore = base.GetRequiredModuleTypesCore();
            requiredModuleTypesCore.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
            return requiredModuleTypesCore;
        }
    }
}