using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using Xpand.ExpressApp.Scheduler.Model;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Win.Controllers {
    public class SchedulerModelAdapterController : SchedulerModelAdapterControllerBase {
        public new SchedulerListEditor SchedulerListEditor {
            get {return base.SchedulerListEditor as SchedulerListEditor;}
        }

        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            return base.CreateBuilderData().Concat(new[]{
                new InterfaceBuilderData(typeof (SchedulerPopupMenu)){
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
            return typeof (SchedulerControl);
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing += SchedulerControlOnPopupMenuShowing;
            }
        }

        protected override void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas) {
            Type extenderType = typeof(SchedulerControl);
            Assembly assembly = builder.Build(interfaceBuilderDatas, GetPath(extenderType.Name));

            builder.ExtendInteface(typeof(IModelOptionsSchedulerEx), extenderType, assembly);
            builder.ExtendInteface<IModelSchedulerPopupMenu, SchedulerPopupMenu>(assembly);
            builder.ExtendInteface<IModelAppoitmentLabel, AppointmentLabel>(assembly);
            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageLabels>(assembly);
            builder.ExtendInteface<IModelAppoitmentStatus, AppointmentStatus>(assembly);
            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageStatuses>(assembly);
        }

        protected override Type[] BaseSchedulerControlTypes() {
            return base.BaseSchedulerControlTypes().Concat(new[]{
                typeof (SchedulerViewTypedRepositoryBase<SchedulerViewBase>), typeof (DayView),
                typeof (TimelineView), typeof (WeekView)
            }).ToArray();
        }

        protected override AppointmentStatusBaseCollection Statuses() {
            if (SchedulerListEditor != null) return SchedulerListEditor.SchedulerControl.Storage.Appointments.Statuses;
            var schedulerStorage = SchedulerStorage();
            return schedulerStorage != null ? schedulerStorage.Appointments.Statuses : new AppointmentStatusCollection();
        }

        protected override AppointmentLabelBaseCollection Labels() {
            if (SchedulerListEditor != null) 
                return SchedulerListEditor.SchedulerControl.Storage.Appointments.Labels;
            var schedulerStorage = SchedulerStorage();
            return schedulerStorage != null ? schedulerStorage.Appointments.Labels : new AppointmentLabelCollection();
        }

        SchedulerStorage SchedulerStorage() {
            var schedulerLabelPropertyEditor = ((DetailView) View).GetItems<SchedulerLabelPropertyEditor>().FirstOrDefault();
            return schedulerLabelPropertyEditor != null ? ((AppointmentLabelEdit) schedulerLabelPropertyEditor.Control).Storage : null;
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
