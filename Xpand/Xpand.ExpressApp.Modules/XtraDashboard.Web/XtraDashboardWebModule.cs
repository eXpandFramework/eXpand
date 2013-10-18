using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using Xpand.ExpressApp.Dashboard;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.XtraDashboard.Web {
    [ToolboxBitmap(typeof(XtraDashboardWebModule))]
    [ToolboxItem(true)]
    [ToolboxTabName(XpandAssemblyInfo.TabAspNetModules)]
    public sealed class XtraDashboardWebModule : XpandModuleBase { 
        public XtraDashboardWebModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
        }
    }
}
