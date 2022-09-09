using System;
using System.Diagnostics.CodeAnalysis;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.XtraScheduler;

namespace Xpand.ExpressApp.Scheduler.Win.Reminders {
    public class RemindersForm : DevExpress.XtraScheduler.Forms.RemindersForm {
        readonly XafApplication _xafApplication;

        public RemindersForm(XafApplication xafApplication, ISchedulerStorage storage)
            : base(storage) {
            _xafApplication = xafApplication;
        }

        [SuppressMessage("Usage", "XAF0022:Avoid calling the ShowViewStrategyBase.ShowView() method")]
        protected override void OpenSelectedItem() {
            var selectedReminders = GetSelectedReminders();
            if (selectedReminders.Count <= 0) return;
            var appointment = selectedReminders[0].Appointment;
            var objectType = (Type) appointment.CustomFields[Scheduler.Reminders.SchedulerStorage.BOType];
            var objectSpace = _xafApplication.CreateObjectSpace(objectType);
            var obj = objectSpace.FindObject(objectType, new BinaryOperator("Oid", appointment.Id));
            var detailView = _xafApplication.CreateDetailView(objectSpace, obj);
            var showViewParameters = new ShowViewParameters(detailView);
            _xafApplication.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(_xafApplication.MainWindow, null));
        }
    }
}
