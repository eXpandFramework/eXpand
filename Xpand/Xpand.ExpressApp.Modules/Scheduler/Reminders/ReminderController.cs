using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;

namespace Xpand.ExpressApp.Scheduler.Reminders{
    public class ReminderController:ObjectViewController<ObjectView,IEvent>{
        private Dictionary<IEvent, Appointment> _appointments = new Dictionary<IEvent, Appointment>();
        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.Committing-=ObjectSpaceOnCommitting;
            ObjectSpace.Committed-=ObjectSpaceOnCommitted;
        }

        protected override void OnActivated(){
            base.OnActivated();
            ObjectSpace.Committing+=ObjectSpaceOnCommitting;
            ObjectSpace.Committed += ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs eventArgs){
            UpdateAppoitmentKey(_appointments);
        }

        internal void UpdateAppoitmentKey(Dictionary<IEvent, Appointment> valuePairs) {
            foreach (var valuePair in valuePairs) {
                var @event = valuePair.Key;
                var typeInfo = Application.TypesInfo.FindTypeInfo(@event.GetType());
                valuePair.Value.CustomFields[SchedulerStorage.BOType] = typeInfo.Type;
                valuePair.Value.CustomFields[SchedulerStorage.BOKey] = typeInfo.KeyMember.GetValue(@event);
            }
        }

        public CriteriaOperator GetCriteria(IModelMemberReminderInfo modelMemberReminderInfo) {
            if (modelMemberReminderInfo != null){
                var modelCriteria = CriteriaOperator.Parse(modelMemberReminderInfo.ReminderCriteria);
                var reminderCriteria = CriteriaOperator.And(
                    new BinaryOperator(modelMemberReminderInfo.Name + ".HasReminder", true),
                    new UnaryOperator(UnaryOperatorType.Not, new NullOperator(modelMemberReminderInfo.Name + ".Info")));
                return CriteriaOperator.And(modelCriteria, reminderCriteria);
            }
            return null;
        }

        private void ObjectSpaceOnCommitting(object sender, CancelEventArgs cancelEventArgs){
            foreach (var evt in ObjectSpace.ModifiedObjects.OfType<IEvent>()) {
                var modelMemberReminderInfo = evt.ModelMemberReminderInfo();
                if (modelMemberReminderInfo != null && evt.GetReminderInfoMemberValue().HasReminder) {
                    _appointments = CreateAppoitments(new[] { evt });
                }    
            }
        }

        private void SetupAppointment(Appointment appointment, IEvent iEvent) {
            appointment.Subject = iEvent.Subject;
            appointment.StatusId = iEvent.Status;
            appointment.Start = iEvent.StartOn;
            appointment.End = iEvent.EndOn;
        }

        private Reminder CreateReminder(IEvent @event, Appointment appointment) {
            var reminderInfo = @event.GetReminderInfoMemberValue();
            appointment.HasReminder = reminderInfo.HasReminder;
            var reminder = appointment.CreateNewReminder();
            reminder.AlertTime = DateTime.Now.Add(reminderInfo.TimeBeforeStart);
            return reminder;
        }

        public Dictionary<IEvent, Appointment> CreateAppoitments(IEnumerable<IEvent> iEvents) {
            var appointments = new Dictionary<IEvent, Appointment>();
            SchedulerStorage.Instance.EnableReminders = false;
            foreach (var iEvent in iEvents) {
                var reminderInfo = iEvent.GetReminderInfoMemberValue();
                var appointment = new Appointment(AppointmentType.Normal, iEvent.StartOn, iEvent.EndOn - iEvent.StartOn, iEvent.Subject, iEvent.AppointmentId);
                SetupAppointment(appointment, iEvent);
                var reminder = CreateReminder(iEvent, appointment);
                appointment.Reminders.RemoveAt(0);
                appointment.Reminders.Add(reminder);
                var helper = new ReminderXmlPersistenceHelper(reminder, DateSavingType.LocalTime);
                reminderInfo.Info = helper.ToXml();
                appointments.Add(iEvent, appointment);
                SchedulerStorage.Instance.Appointments.Add(appointment);
            }
            SchedulerStorage.Instance.EnableReminders = true;
            SchedulerStorage.Instance.TriggerAlerts();
            return appointments;
        }
    }
}