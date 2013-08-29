using System;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.Utils.Menu;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.ExpressApp.Scheduler.Model;
using Xpand.Persistent.Base.ModelAdapter;
using PopupMenuShowingEventArgs = DevExpress.Web.ASPxScheduler.PopupMenuShowingEventArgs;
using System.Linq;

namespace Xpand.ExpressApp.Scheduler.Web.Controllers {
    public class SchedulerModelAdapterController : SchedulerModelAdapterControllerBase {
        public new ASPxSchedulerListEditor SchedulerListEditor {
            get {
                return base.SchedulerListEditor as ASPxSchedulerListEditor;
            }
        }

        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            return base.CreateBuilderData().Concat(new[]{
                new InterfaceBuilderData(typeof (ASPxSchedulerPopupMenu)){
                    Act = info => info.Name != "Id" && info.DXFilter(typeof (DXMenuItem))
                },
                new InterfaceBuilderData(typeof (AppointmentLabel)){
                    Act = info => info.DXFilter(typeof (UserInterfaceObject))
                },
                new InterfaceBuilderData(typeof (AppointmentStatus)){
                    Act = info => info.DXFilter(typeof (UserInterfaceObject))
                }
            });
        }

        protected override Type SchedulerControlType() {
            return typeof (ASPxScheduler);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing += SchedulerControlOnPopupMenuShowing;
            }
        }

        protected override void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas) {
            Type extenderType = typeof (ASPxScheduler);
            Assembly assembly = builder.Build(interfaceBuilderDatas, GetPath(extenderType.Name));

            builder.ExtendInteface(typeof (IModelOptionsSchedulerEx), extenderType, assembly);
            builder.ExtendInteface<IModelSchedulerPopupMenu, ASPxSchedulerPopupMenu>(assembly);
            builder.ExtendInteface<IModelAppoitmentLabel, AppointmentLabel>(assembly);
            builder.ExtendInteface<ASPxAppointmentStorage, IAppoitmentStorageLabels>(assembly);
            builder.ExtendInteface<IModelAppoitmentStatus, AppointmentStatus>(assembly);
            builder.ExtendInteface<ASPxAppointmentStorage, IAppoitmentStorageStatuses>(assembly);
        }

        protected override Type[] BaseSchedulerControlTypes() {
            return base.BaseSchedulerControlTypes().Concat(new[]{
                typeof (SchedulerViewTypedRepositoryBase<SchedulerViewBase>), typeof (DayView),
                typeof (TimelineView), typeof (WeekView)
            }).ToArray();
        }

        protected override AppointmentStatusBaseCollection Statuses() {
            return SchedulerListEditor.SchedulerControl.Storage.Appointments.Statuses;
        }

        protected override AppointmentLabelBaseCollection Labels() {
            return SchedulerListEditor.SchedulerControl.Storage.Appointments.Labels;
        }

        protected override IInnerSchedulerControlOwner SchedulerControl() {
            return SchedulerListEditor.SchedulerControl;
        }

        protected override ResourceStorageBase Resources() {
            return SchedulerListEditor.SchedulerControl.Storage.Resources;
        }

        protected override IEnumerable<Appointment> Items() {
            return SchedulerListEditor.SchedulerControl.Storage.Appointments.Items;
        }

        protected override SchedulerStorageBase Storage() {
            return SchedulerListEditor.SchedulerControl.Storage;
        }

        void SchedulerControlOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
            SynchMenu(e.Menu);
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing -= SchedulerControlOnPopupMenuShowing;
            }
        }

    }
}