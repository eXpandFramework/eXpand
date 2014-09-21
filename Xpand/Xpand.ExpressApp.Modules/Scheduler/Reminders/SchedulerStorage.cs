using DevExpress.Persistent.Base;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Reminders{
    public class SchedulerStorage{
        public const string BOType = "BOType";
        public const string BOKey = "BOKey";
        private static readonly object _syncRoot = new object();
        private const string ValueManagerKey = "SchedulerStorage";
        private static volatile IValueManager<DevExpress.XtraScheduler.SchedulerStorage> _instanceValueManager;
        

        private SchedulerStorage(){
        }

        static DevExpress.XtraScheduler.SchedulerStorage InitSchedulerStorage() {
            var modelApplication = ApplicationHelper.Instance.Application.Model;
            var storage = new DevExpress.XtraScheduler.SchedulerStorage();
            storage.BeginInit();
            storage.EnableReminders = true;
            storage.RemindersCheckInterval = (((IModelApplicationScheduler)modelApplication).Scheduler).RemindersCheckInterval;
            var appointmentStorage = storage.Appointments;
            appointmentStorage.AutoReload = false;
            var mappingCollection = appointmentStorage.CustomFieldMappings;
            mappingCollection.Add(new AppointmentCustomFieldMapping(BOType, BOType));
            mappingCollection.Add(new AppointmentCustomFieldMapping(BOKey, BOKey));
            storage.EndInit();
            return storage;
        }

        public static DevExpress.XtraScheduler.SchedulerStorage Instance {
            get {
                if (_instanceValueManager == null) {
                    lock (_syncRoot) {
                        if (_instanceValueManager == null) {
                            _instanceValueManager = ValueManager.GetValueManager<DevExpress.XtraScheduler.SchedulerStorage>(ValueManagerKey);
                        }
                    }
                }
                if (_instanceValueManager.Value == null) {
                    lock (_syncRoot) {
                        if (_instanceValueManager.Value == null) {
                            _instanceValueManager.Value = InitSchedulerStorage();
                        }
                    }
                }
                return _instanceValueManager.Value;
            }
        }

    }
}