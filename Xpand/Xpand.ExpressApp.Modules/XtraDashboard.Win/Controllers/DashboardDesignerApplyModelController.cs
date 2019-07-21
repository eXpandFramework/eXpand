using DevExpress.ExpressApp;
using Xpand.ExpressApp.Dashboard;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper;
using Xpand.XAF.Modules.ModelMapper.Services;

namespace Xpand.ExpressApp.XtraDashboard.Win.Controllers {
    public class DashboardDesignerApplyModelController:ViewController {
        protected override void OnActivated(){
            base.OnActivated();
            Frame.GetController<DashboardDesignerController>(controller => controller.DashboardDesignerOpening += OnDashboardDesignerOpening);
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Frame.GetController<DashboardDesignerController>(controller => controller.DashboardDesignerOpening -= OnDashboardDesignerOpening);
        }

        private void OnDashboardDesignerOpening(object sender, Dashboard.Controllers.DashboardDesignerOpeningEventArgs e) {
            var args = ((DashboardDesignerOpeningEventArgs) e);
            var settings = (IModelModelMap)( ((IModelApplicationDashboardModule) Application.Model).DashboardModule).GetNode("DesignerSettings");
            settings.BindTo(args.Designer);
        }
    }
}
