using System.Diagnostics.CodeAnalysis;
using DevExpress.ExpressApp;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    public class ReminderAlertController : Scheduler.Reminders.ReminderAlertController {

        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        protected override void ShowReminderAlerts(object sender, ReminderEventArgs e) {
            var view = (DashboardView)Application.CreateView(Application.FindModelView("ReminderFormView"));
            var showViewParameters = new ShowViewParameters(view){TargetWindow = TargetWindow.NewWindow};
            Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Application.MainWindow, null));            
        }
    }
}
