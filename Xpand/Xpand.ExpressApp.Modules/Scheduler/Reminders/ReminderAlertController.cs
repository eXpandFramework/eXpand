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
using DevExpress.XtraScheduler.Native;
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
        private bool _firstViewShown;

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

        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            Application.ViewShown+=ApplicationOnViewShown;
        }

        private void ApplicationOnViewShown(object sender, ViewShownEventArgs viewShownEventArgs){
            _firstViewShown = true;
            Application.ViewShown -= ApplicationOnViewShown;
        }

        private void RestoreAppointments(){
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            var task = Task.Factory.StartNew(() =>{
                while (!_firstViewShown){
                    Thread.Sleep(100);
                }
                Thread.Sleep(2000);
            });
            task.ContinueWith(task1 =>{
                var reminderInfos = Application.TypesInfo.PersistentTypes.Select(ReminderMembers).Where(info => info != null);
                var reminderController = Frame.GetController<ReminderController>();
                var objectSpace = Application.CreateObjectSpace();
                foreach (var modelMemberReminderInfo in reminderInfos) {
                    var criteriaOperator = reminderController.GetCriteria(modelMemberReminderInfo);
                    var reminderEvents = objectSpace.GetObjects(modelMemberReminderInfo.ModelClass.TypeInfo.Type, criteriaOperator, false).Cast<IEvent>();
                    var appointments = reminderController.CreateAppoitments(reminderEvents);
                    reminderController.UpdateAppoitmentKey(appointments);
                }
            }, context);
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
                reminderInfo.Info = !reminderInfo.HasReminder ? null : new ReminderXmlPersistenceHelper(appointment.Reminder, new TimeZoneEngine()).ToXml();
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
