using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using System.Linq;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public class ReminderInfoRefreshController : ViewController<DetailView> {
        IEvent _event;
        IMemberInfo _reminderMember;

        public ReminderInfoRefreshController() {
            TargetObjectType = typeof(IEvent);
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (!RemindersEnabled())
                return;
            _event = (IEvent) View.CurrentObject;
            _reminderMember = _event.ModelMemberReminderInfo().MemberInfo;
            View.ObjectSpace.Committing += ObjectSpace_Committing;
        }

        ReminderInfo GetReminderInfo() {
            return (ReminderInfo) _reminderMember.GetValue(View.CurrentObject);
        }

        bool RemindersEnabled() {
            return View.ObjectTypeInfo.FindAttribute<SupportsReminderAttribute>()!=null;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (RemindersEnabled()) {
                View.ObjectSpace.Committing -= ObjectSpace_Committing;
            }
        }

        void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (ObjectSpace.ModifiedObjects.OfType<ReminderInfo>().Any())
                UpdateReminderInfo();
        }

        private Appointment CreateAppointment() {
            var appointment = new Appointment(AppointmentType.Normal) {
                Subject = _event.Subject,
                Start = _event.StartOn,
                End = _event.EndOn,
                HasReminder = GetReminderInfo().HasReminder
            };
            return appointment;
        }

        private void UpdateReminderInfo() {
            var reminderInfo = GetReminderInfo();
            if (!reminderInfo.HasReminder)
                reminderInfo.Info = null;
            else {
                var appointment = CreateAppointment();
                var reminder = appointment.CreateNewReminder();
                reminder.AlertTime = DateTime.Now.Add(reminderInfo.TimeBeforeStart);
                var helper = new ReminderXmlPersistenceHelper(reminder, DateSavingType.LocalTime);
                reminderInfo.Info = helper.ToXml();
            }
        }
    }
}
