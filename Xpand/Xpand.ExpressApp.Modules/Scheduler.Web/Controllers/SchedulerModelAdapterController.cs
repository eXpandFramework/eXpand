using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler.Web;
using DevExpress.Web.ASPxScheduler;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.ExpressApp.Scheduler.Model;
using Xpand.Persistent.Base.General;
using PopupMenuShowingEventArgs = DevExpress.Web.ASPxScheduler.PopupMenuShowingEventArgs;

namespace Xpand.ExpressApp.Scheduler.Web.Controllers {
    public class SchedulerMenuItemAdapterController : ViewController, IModelExtender {
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
//            var interfaceBuilder = new InterfaceBuilder(extenders);
//            var componentType = typeof(DXMenuItem);
//            var builderData = new InterfaceBuilderData(componentType) {
//                Act = info => info.Name != "Id" && info.DXFilter()
//            };
//            var assembly = interfaceBuilder.Build(new List<InterfaceBuilderData> { builderData },GetPath(componentType.Name));
//
//            interfaceBuilder.ExtendInteface<IModelSchedulerPopupMenuItem, DXMenuItem>(assembly);
        }
    }

    public class SchedulerModelAdapterController : SchedulerModelAdapterControllerBase {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing += SchedulerControlOnPopupMenuShowing;
            }

        }
        public new ASPxSchedulerListEditor SchedulerListEditor => base.SchedulerListEditor as ASPxSchedulerListEditor;

//        protected override IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            return base.CreateBuilderData().Concat(new[]{
//                new InterfaceBuilderData(typeof (ASPxSchedulerPopupMenu)){
//                    Act = info => info.Name != "Id" && info.DXFilter(typeof (DXMenuItem))
//                },
//                new InterfaceBuilderData(typeof (AppointmentLabel)){
//                    Act = info => info.DXFilter(typeof (UserInterfaceObject))
//                },
//                new InterfaceBuilderData(typeof (AppointmentStatus)){
//                    Act = info => info.DXFilter(typeof (UserInterfaceObject))
//                }
//            });
//        }

//        protected override Type SchedulerControlType() {
//            return typeof (ASPxScheduler);
//        }

//        protected override void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas) {
////            Type extenderType = typeof (ASPxScheduler);
////            Assembly assembly = builder.Build(interfaceBuilderDatas, GetPath(extenderType.Name));
////            throw new NotImplementedException();
//////            builder.ExtendInteface(typeof (IModelOptionsSchedulerEx), extenderType, assembly);
//////            builder.ExtendInteface<IModelAppoitmentLabel, AppointmentLabel>(assembly);
////            builder.ExtendInteface<ASPxAppointmentStorage, IAppoitmentStorageLabels>(assembly);
////            builder.ExtendInteface<IModelAppoitmentStatus, AppointmentStatus>(assembly);
////            builder.ExtendInteface<ASPxAppointmentStorage, IAppoitmentStorageStatuses>(assembly);
//        }

//        protected override Type[] BaseSchedulerControlTypes() {
//            return base.BaseSchedulerControlTypes().Concat(new[]{
//                typeof (SchedulerViewTypedRepositoryBase<SchedulerViewBase>), typeof (DayView),
//                typeof (TimelineView), typeof (WeekView)
//            }).ToArray();
//        }

        protected override IAppointmentStatusStorage Statuses() {
            return SchedulerListEditor.SchedulerControl.Storage.Appointments.Statuses;
        }

        protected override IAppointmentLabelStorage Labels() {
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

        protected override ISchedulerStorageBase Storage() {
            return SchedulerListEditor.SchedulerControl.Storage;
        }

        void SchedulerControlOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
            SynchMenu(e.Menu);
        }

        protected override object GetMenu(object popupMenu, IModelNode modelMenu){
            return ((ASPxSchedulerPopupMenu) popupMenu).Items.FindByText(modelMenu.Id());
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor?.SchedulerControl != null)
                SchedulerListEditor.SchedulerControl.PopupMenuShowing -= SchedulerControlOnPopupMenuShowing;
        }
    }
}