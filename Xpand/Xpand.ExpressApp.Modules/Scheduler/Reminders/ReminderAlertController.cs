using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public abstract class ReminderAlertController : WindowController {
        public const string BoTypeCustomField = "BOType";
        SchedulerStorage _schedulerStorage;
        Timer _dataSourceRefreshTimer;
        AppointmentStorage _appointmentStorage;
        private IObjectSpace _objectSpace;

        protected ReminderAlertController() {
            TargetWindowType = WindowType.Main;
        }

        void Frame_TemplateChanged(object sender, EventArgs e) {
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Frame.ViewChanged += Frame_ViewChanged;
            }
        }

        void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
            Frame.ViewChanged -= Frame_ViewChanged;
            _dataSourceRefreshTimer.Start();
            RefreshReminders();
        }

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            _objectSpace = Application.CreateObjectSpace();
            Frame.TemplateChanged += Frame_TemplateChanged;
            Frame.Disposing+=FrameOnDisposing;
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            _objectSpace.Dispose();
            Frame.Disposing-=FrameOnDisposing;
            Frame.TemplateChanged -= Frame_TemplateChanged;
        }

        protected override void OnActivated() {
            base.OnActivated();
            InitScheduler();
            InitDataSourceRefreshTimer();
        }

        protected override void OnDeactivated() {
            _dataSourceRefreshTimer.Stop();
            _dataSourceRefreshTimer.Elapsed -= RefreshTimerElapsed;
            _schedulerStorage.AppointmentChanging -= SchedulerStorageAppointmentChanging;
            _schedulerStorage.ReminderAlert -= ShowReminderAlerts;
            _dataSourceRefreshTimer = null;
            _schedulerStorage = null;
            base.OnDeactivated();
        }

        private void InitDataSourceRefreshTimer() {
            _dataSourceRefreshTimer = new Timer { Interval = 5000 };
            _dataSourceRefreshTimer.Elapsed += RefreshTimerElapsed;
        }

        protected virtual void InitScheduler() {
            _schedulerStorage = new SchedulerStorage();
            _schedulerStorage.BeginInit();
            _schedulerStorage.EnableReminders = true;
            _schedulerStorage.RemindersCheckInterval =( ((IModelApplicationScheduler) Application.Model).Scheduler).RemindersCheckInterval;
            _appointmentStorage = (_schedulerStorage).Appointments;
            _appointmentStorage.AutoReload = false;
            var mappingCollection = _appointmentStorage.CustomFieldMappings;
            mappingCollection.Add(new AppointmentCustomFieldMapping(BoTypeCustomField, BoTypeCustomField));
            _schedulerStorage.AppointmentChanging += SchedulerStorageAppointmentChanging;
            _schedulerStorage.ReminderAlert += ShowReminderAlerts;
            _schedulerStorage.EndInit();
        }

        protected virtual void ShowReminderAlerts(object sender, ReminderEventArgs e) { }

        void RefreshTimerElapsed(object sender, ElapsedEventArgs e) {
            RefreshReminders();
        }

        private void StopRefreshingStorage() {
            _dataSourceRefreshTimer.Stop();
            _dataSourceRefreshTimer.Elapsed -= RefreshTimerElapsed;
        }

        private void StartRefreshingStorage() {
            _dataSourceRefreshTimer.Elapsed += RefreshTimerElapsed;
            _dataSourceRefreshTimer.Start();
        }

        private void BeginAppointmentsUpdating() {
            _schedulerStorage.AppointmentChanging -= SchedulerStorageAppointmentChanging;
            _appointmentStorage.BeginUpdate();
        }

        private void EndAppointmentsUpdating() {
            _appointmentStorage.EndUpdate();
            _schedulerStorage.TriggerAlerts();
            _schedulerStorage.AppointmentChanging += SchedulerStorageAppointmentChanging;
        }

        void SchedulerStorageAppointmentChanging(object sender, PersistentObjectCancelEventArgs e) {
            StopRefreshingStorage();
            var appointment = e.Object as Appointment;
            if (appointment == null) throw new NullReferenceException("appointment");
            var type = (Type) appointment.CustomFields[BoTypeCustomField];
            var eventBO = ((IEvent)_objectSpace.GetObjectByKey(type, appointment.Id));
            var reminderInfo = eventBO.GetReminderInfoMemberValue();
            reminderInfo.HasReminder = appointment.HasReminder;
            reminderInfo.Info = !reminderInfo.HasReminder ? null : new ReminderXmlPersistenceHelper(appointment.Reminder, DateSavingType.LocalTime).ToXml();
            _objectSpace.CommitChanges();
            StartRefreshingStorage();
        }

        private void RefreshReminders() {
            StopRefreshingStorage();
            BeginAppointmentsUpdating();

            var currentAppointmentKeys = new List<Guid>();
            foreach (var modelMemberReminderInfo in Application.TypesInfo.PersistentTypes.Select(ReminderMembers).Where(info => info != null)) {
                var criteriaOperator = ExtractCriteria(modelMemberReminderInfo);
                var reminderEvents = _objectSpace.GetObjects(modelMemberReminderInfo.ModelClass.TypeInfo.Type, criteriaOperator, false).Cast<IEvent>();
                foreach (IEvent evt in reminderEvents) {
                    UpdateAppointmentInStorage(modelMemberReminderInfo.ModelClass.TypeInfo, evt);
                    currentAppointmentKeys.Add((Guid)evt.AppointmentId);
                }
            }
            RemoveRedundantAppointments(currentAppointmentKeys);
            EndAppointmentsUpdating();
            StartRefreshingStorage();
        }

        IModelMemberReminderInfo ReminderMembers(ITypeInfo info) {
            return info.Implements<IEvent>() ? info.ModelMemberReminderInfo(Application.Model.BOModel) : null;
        }

        private void RemoveRedundantAppointments(IList<Guid> refreshedReminderOid) {
            for (var i = _appointmentStorage.Count - 1; i >= 0; i--) {
                if (!refreshedReminderOid.Contains((Guid)_appointmentStorage[i].Id))
                    _appointmentStorage.Remove(_appointmentStorage[i]);
            }
        }

        private void UpdateAppointmentInStorage(ITypeInfo typeInfo, IEvent iEvent) {
            _schedulerStorage.EnableReminders = false;
            var appointment = _appointmentStorage.GetAppointmentById(iEvent.AppointmentId);
            if (appointment == null) {
                appointment = new Appointment(AppointmentType.Normal, iEvent.StartOn, iEvent.EndOn - iEvent.StartOn, iEvent.Subject, iEvent.AppointmentId);
                _appointmentStorage.Add(appointment);
            }
            appointment.Subject = iEvent.Subject;
            appointment.StatusId = iEvent.Status;
            appointment.Start = iEvent.StartOn;
            appointment.End = iEvent.EndOn;
            var reminderInfo = iEvent.GetReminderInfoMemberValue();
            appointment.HasReminder = reminderInfo.HasReminder;
            appointment.CustomFields[BoTypeCustomField] = typeInfo.Type;

            var reminder = appointment.CreateNewReminder();
            var reminderHelper = new ReminderXmlPersistenceHelper(reminder, DateSavingType.LocalTime);
            var value = (Reminder) reminderHelper.FromXml(reminderInfo.Info);
            appointment.Reminders.Add(value);
            _schedulerStorage.EnableReminders = true;
        }

        private CriteriaOperator ExtractCriteria(IModelMemberReminderInfo modelMemberReminderInfo) {
            var modelCriteria = CriteriaOperator.Parse(modelMemberReminderInfo.ReminderCriteria);
            var reminderCriteria = CriteriaOperator.And(
                        new BinaryOperator(modelMemberReminderInfo.Name+ ".HasReminder", true),
                        new UnaryOperator(UnaryOperatorType.Not, new NullOperator(modelMemberReminderInfo.Name + ".Info")));
            return CriteriaOperator.And(modelCriteria, reminderCriteria);
        }

        public SchedulerStorageBase Storage {
            get { return _schedulerStorage; }
        }
    }
}
