using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers {
    public class WebNewDashboardController:DevExpress.ExpressApp.Dashboards.Web.WebNewDashboardController {
        protected override void ProcessObjectCreating(ObjectCreatingEventArgs e) {
            if (!typeof(IDashboardDefinition).IsAssignableFrom(e.ObjectType))
                base.ProcessObjectCreating(e);
        }

    }
}
