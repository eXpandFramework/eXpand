using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers {
    public class WebEditDashboardController:DevExpress.ExpressApp.Dashboards.Web.WebEditDashboardController {
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
