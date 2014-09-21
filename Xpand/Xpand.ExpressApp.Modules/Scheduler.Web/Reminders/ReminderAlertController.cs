using DevExpress.ExpressApp;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    public class ReminderAlertController : Scheduler.Reminders.ReminderAlertController {

        protected override void ShowReminderAlerts(object sender, ReminderEventArgs e) {
            var view = (DashboardView)Application.CreateView(Application.FindModelView("ReminderFormView"));
            var showViewParameters = new ShowViewParameters(view){TargetWindow = TargetWindow.NewWindow};
            Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Application.MainWindow, null));            
        }
    }
}
