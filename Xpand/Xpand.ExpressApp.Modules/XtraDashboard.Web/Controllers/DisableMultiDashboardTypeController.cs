using System;
using DevExpress.Web;
using Xpand.ExpressApp.XtraDashboard.Web.PropertyEditors;

namespace Xpand.ExpressApp.XtraDashboard.Web.Controllers {
    public class DisableMultiDashboardTypeController : Dashboard.Controllers.DisableMultiDashboardTypeController {
        protected override void OnControlCreated(object sender, EventArgs e) {
            ((DashboardTypesEditor) sender).ListBoxTemplate.SelectionMode = ListEditSelectionMode.Single;
        }
    }
}