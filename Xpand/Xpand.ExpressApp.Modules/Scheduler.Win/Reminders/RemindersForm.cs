using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.XtraScheduler;

namespace Xpand.ExpressApp.Scheduler.Win.Reminders {
    public class RemindersForm : DevExpress.XtraScheduler.Forms.RemindersForm {
        readonly XafApplication _xafApplication;

        public RemindersForm(XafApplication xafApplication, SchedulerStorage storage)
            : base(storage) {
            _xafApplication = xafApplication;
        }

        protected override void OpenSelectedItem() {
            var selectedReminders = GetSelectedReminders();
            if (selectedReminders.Count <= 0) return;
            var appointment = selectedReminders[0].Appointment;
            var objectSpace = _xafApplication.CreateObjectSpace();
            var objectType = (Type) appointment.CustomFields[Scheduler.Reminders.ReminderAlertController.BoTypeCustomField];
            var obj = objectSpace.FindObject(objectType, new BinaryOperator("Oid", appointment.Id));
            var detailView = _xafApplication.CreateDetailView(objectSpace, obj);
            var showViewParameters = new ShowViewParameters(detailView);
            _xafApplication.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(_xafApplication.MainWindow, null));
        }
    }
}
