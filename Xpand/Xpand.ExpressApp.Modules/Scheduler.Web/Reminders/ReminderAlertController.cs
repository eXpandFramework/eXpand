using System;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Web.Reminders {
    public class ReminderAlertController : Scheduler.Reminders.ReminderAlertController {

        protected override void ShowReminderAlerts(object sender, ReminderEventArgs e) {
            var view = (DashboardView)Application.CreateView(Application.FindModelView("ReminderFormView"));
            var showViewParameters = new ShowViewParameters(view);
            var dashboardViewItems = view.GetItems<DashboardViewItem>().First();
            showViewParameters.TargetWindow = TargetWindow.NewWindow;
            dashboardViewItems.ControlCreated += DashboardViewItemsOnControlCreated;
            Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Application.MainWindow, null));            
        }

        void DashboardViewItemsOnControlCreated(object sender, EventArgs eventArgs) {
            
        }


    }
}
