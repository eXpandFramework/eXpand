using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class WinNewDashboardController:DevExpress.ExpressApp.Dashboards.Win.WinNewDashboardController {
        protected override void ProcessObjectCreating(ObjectCreatingEventArgs e) {
            if (!typeof(IDashboardDefinition).IsAssignableFrom(e.ObjectType))
                base.ProcessObjectCreating(e);
        }
    }
}
