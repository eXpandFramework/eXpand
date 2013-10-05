using Xpand.ExpressApp.Dashboard;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.XtraDashboard.Web {
    public sealed class XtraDashboardWebModule : XpandModuleBase { 
        public XtraDashboardWebModule() {
            RequiredModuleTypes.Add(typeof(DashboardModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule));
        }
    }
}
