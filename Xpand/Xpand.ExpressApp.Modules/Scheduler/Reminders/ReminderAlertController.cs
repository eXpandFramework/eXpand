using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Xml;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Scheduler.Reminders {
    public interface IModelScheduler : IModelNode {
        [DefaultValue(1000)]
        int RemindersCheckInterval { get; set; }
    }
    public interface IModelApplicationScheduler {
        IModelScheduler Scheduler { get; }
    }

    public abstract class ReminderAlertController : WindowController, IModelExtender {
        private IObjectSpace _objectSpace;

        protected ReminderAlertController() {
            TargetWindowType = WindowType.Main;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0012:Avoid calling the XafApplication.CreateObjectSpace() method without Type parameter", Justification = "<Pending>")]
        protected override void OnActivated() {
            base.OnActivated();
            _objectSpace = Application.CreateObjectSpace();
            SchedulerStorage.Instance.AppointmentChanging += SchedulerStorageAppointmentChanging;
            SchedulerStorage.Instance.ReminderAlert += ShowReminderAlerts;
        }

        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow) {
                Application.ViewShown += ApplicationOnViewShown;
                Frame.Disposing += FrameOnDisposing;
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs) {
            Frame.Disposing -= FrameOnDisposing;
            Application.ViewShown -= ApplicationOnViewShown;
        }

        private void ApplicationOnViewShown(object sender, ViewShownEventArgs viewShownEventArgs) {

            Task.Factory.StartNew(() => {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (stopwatch.Elapsed.TotalMilliseconds < 2000) {
                    Thread.Sleep(100);
                }
                stopwatch.Stop();
            }).ContinueWith(task => {
                CreateAppoitments();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            ((XafApplication)sender).ViewShown -= ApplicationOnViewShown;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0012:Avoid calling the XafApplication.CreateObjectSpace() method without Type parameter", Justification = "<Pending>")]
        private void CreateAppoitments() {
            if (Application != null){
                var reminderInfos = Application.TypesInfo.PersistentTypes.Select(ReminderMembers).Where(info => info != null);
                Frame.GetController<ReminderController>(controller => {
                    var objectSpace = Application.CreateObjectSpace();
                    foreach (var modelMemberReminderInfo in reminderInfos) {
                        var criteriaOperator = controller.GetCriteria(modelMemberReminderInfo);
                        var reminderEvents =
                            objectSpace.GetObjects(modelMemberReminderInfo.ModelClass.TypeInfo.Type, criteriaOperator, false)
                                .Cast<IEvent>();
                        var appointments = controller.CreateAppoitments(reminderEvents);
                        controller.UpdateAppoitmentKey(appointments);
                    }
                });
            }
        }


        protected override void OnDeactivated() {
            base.OnDeactivated();
            _objectSpace.Dispose();
            SchedulerStorage.Instance.AppointmentChanging -= SchedulerStorageAppointmentChanging;
            SchedulerStorage.Instance.ReminderAlert -= ShowReminderAlerts;
        }

        IModelMemberReminderInfo ReminderMembers(ITypeInfo info) {
            return info.Implements<IEvent>() ? info.ModelMemberReminderInfo(Application.Model.BOModel) : null;
        }

        protected abstract void ShowReminderAlerts(object sender, ReminderEventArgs e);

        private void SchedulerStorageAppointmentChanging(object sender, PersistentObjectCancelEventArgs e) {
            var appointment = (Appointment)e.Object;
            var type = (Type)appointment.CustomFields[SchedulerStorage.BOType];
            var key = appointment.CustomFields[SchedulerStorage.BOKey];
            var objectSpace = GetObjectSpace();
            var eventBO = ((IEvent)objectSpace.GetObjectByKey(type, key));
            if (eventBO != null) {
                var reminderInfo = eventBO.GetReminderInfoMemberValue();
                reminderInfo.HasReminder = appointment.HasReminder;
                reminderInfo.Info = !reminderInfo.HasReminder ? null : new ReminderXmlPersistenceHelper(appointment.Reminder).ToXml();
                objectSpace.CommitChanges();
            }
        }

        private IObjectSpace GetObjectSpace() {
            var objectView = Frame.View as ObjectView;
            return objectView != null && objectView.ObjectTypeInfo.Implements<IEvent>()
                ? objectView.ObjectSpace : _objectSpace;
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelApplication, IModelApplicationScheduler>();
        }
    }
}
