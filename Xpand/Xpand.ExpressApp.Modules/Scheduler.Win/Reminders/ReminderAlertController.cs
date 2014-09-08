using System;
using DevExpress.XtraScheduler;

namespace Xpand.ExpressApp.Scheduler.Win.Reminders {
    public class ReminderAlertController : Scheduler.Reminders.ReminderAlertController {
        RemindersForm _remindersForm;

        protected override void ShowReminderAlerts(object sender, ReminderEventArgs e) {
            if (_remindersForm == null) {
                _remindersForm = new RemindersForm(Application, (SchedulerStorage)sender);
                _remindersForm.Disposed += RemindersFormDisposed;
            }
            ((System.Windows.Forms.Form)Application.MainWindow.Template).Invoke((Action)(() => _remindersForm.OnReminderAlert(e)));
        }

        void RemindersFormDisposed(object sender, EventArgs e) {
            ((RemindersForm)sender).Disposed -= RemindersFormDisposed;
            _remindersForm = null;
        }
    }
}
