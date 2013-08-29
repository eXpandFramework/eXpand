using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Win {
    [ToolboxBitmap(typeof(XpandSchedulerWindowsFormsModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandSchedulerWindowsFormsModule : XpandModuleBase {
        public XpandSchedulerWindowsFormsModule() {
            RequiredModuleTypes.Add(typeof(XpandSchedulerModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Win.SchedulerWindowsFormsModule));
        }
    }
}