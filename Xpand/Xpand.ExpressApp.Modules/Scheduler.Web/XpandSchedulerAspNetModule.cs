using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard.Web;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Web {
    [ToolboxBitmap(typeof(XpandSchedulerAspNetModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabWinModules)]
    public sealed class XpandSchedulerAspNetModule : XpandModuleBase {
        public XpandSchedulerAspNetModule() {
            RequiredModuleTypes.Add(typeof (XpandSchedulerModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule));
            RequiredModuleTypes.Add(typeof(DashboardAspNetModule));
        }

    }
}