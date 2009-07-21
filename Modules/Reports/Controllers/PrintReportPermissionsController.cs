using DevExpress.ExpressApp;
using eXpand.ExpressApp.Reports.Security;
using eXpand.ExpressApp.SystemModule;

namespace eXpand.ExpressApp.Reports.Controllers
{
    public abstract class PrintReportPermissionsController<PrintReportViewController> : BaseViewController where PrintReportViewController : ViewController
    {
        protected PrintReportPermissionsController()
        {
            
            TargetObjectType = typeof (PrintPermission);
            TargetViewType=ViewType.DetailView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            View.ControlsCreated +=
                (sender, args) =>
                ((PrintPermission) View.CurrentObject).ControllerType =typeof(PrintReportViewController).FullName;
        }
    }
}