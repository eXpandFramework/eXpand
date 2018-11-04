using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class WinEditDashboardController:DevExpress.ExpressApp.Dashboards.Win.WinEditDashboardController {
        protected override void OnActivated() {
            base.OnActivated();
            ShowDesignerAction.Active[GetType().FullName] = !typeof(IDashboardDefinition).IsAssignableFrom(View.ObjectTypeInfo.Type);
                
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ShowDesignerAction.Active[GetType().FullName] = true;
        }
    }
}
