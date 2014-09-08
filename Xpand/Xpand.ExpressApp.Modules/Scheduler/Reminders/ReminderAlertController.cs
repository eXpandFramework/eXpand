using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public interface IModelScheduler : IModelNode {
        [DefaultValue(1000)]
        int RemindersCheckInterval { get; set; }
    }
    public interface IModelApplicationScheduler {
        IModelScheduler Scheduler { get; }
    }

    public abstract class ReminderAlertController:WindowController,IModelExtender{
        private IObjectSpace _objectSpace;
        
        protected ReminderAlertController(){
            TargetWindowType=WindowType.Main;
        }

        protected override void OnActivated(){
            base.OnActivated();
            _objectSpace = Application.CreateObjectSpace();
            SchedulerStorage.Instance.AppointmentChanging += SchedulerStorageAppointmentChanging;
            SchedulerStorage.Instance.ReminderAlert += ShowReminderAlerts;
            RestoreAppointments();
        }

        private void RestoreAppointments(){
            var task = Task.Factory.StartNew(() => Thread.Sleep(5000));
            task.ContinueWith(task1 =>{
                var reminderInfos = Application.TypesInfo.PersistentTypes.Select(ReminderMembers).Where(info => info != null);
                var reminderController = Frame.GetController<ReminderController>();
                foreach (var modelMemberReminderInfo in reminderInfos){
                    var criteriaOperator = reminderController.GetCriteria(modelMemberReminderInfo);
                    var reminderEvents =_objectSpace.GetObjects(modelMemberReminderInfo.ModelClass.TypeInfo.Type, criteriaOperator, false).Cast<IEvent>();
                    var appointments = reminderController.CreateAppoitments(reminderEvents);
                    reminderController.UpdateAppoitmentKey(appointments);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            _objectSpace.Dispose();
            SchedulerStorage.Instance.AppointmentChanging -= SchedulerStorageAppointmentChanging;
            SchedulerStorage.Instance.ReminderAlert -= ShowReminderAlerts;
        }

        IModelMemberReminderInfo ReminderMembers(ITypeInfo info) {
            return info.Implements<IEvent>() ? info.ModelMemberReminderInfo(Application.Model.BOModel) : null;
        }

        protected abstract void ShowReminderAlerts(object sender, ReminderEventArgs e);

        private void SchedulerStorageAppointmentChanging(object sender, PersistentObjectCancelEventArgs e){
            var appointment = (Appointment)e.Object;
            var type = (Type)appointment.CustomFields[SchedulerStorage.BOType];
            var key = appointment.CustomFields[SchedulerStorage.BOKey];
            var objectSpace = GetObjectSpace();
            var eventBO = ((IEvent)objectSpace.GetObjectByKey(type, key));
            if (eventBO != null){
                var reminderInfo = eventBO.GetReminderInfoMemberValue();
                reminderInfo.HasReminder = appointment.HasReminder;
                reminderInfo.Info = !reminderInfo.HasReminder ? null : new ReminderXmlPersistenceHelper(appointment.Reminder, DateSavingType.LocalTime).ToXml();
                objectSpace.CommitChanges();
            }
        }

        private IObjectSpace GetObjectSpace(){
            var objectView = Frame.View as ObjectView;
            return objectView != null && objectView.ObjectTypeInfo.Implements<IEvent>()
                ? objectView.ObjectSpace: _objectSpace;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelApplication, IModelApplicationScheduler>();
        }
    }
}
