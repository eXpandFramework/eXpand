using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using Fasterflect;
using Xpand.ExpressApp.Scheduler.Model;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Scheduler.Win.Controllers {
    [KeyProperty("MenuId")]
    [DisplayProperty("MenuId")]
    public interface IModelSchedulerPopupMenuItemEx {
        [Required]
        [DataSourceProperty("Menus")]
        string MenuId { get; set; }
        [Browsable(false)]
        IEnumerable<string> Menus { get; } 
    }

    [DomainLogic(typeof(IModelSchedulerPopupMenuItemEx))]
    public class ModelSchedulerPopupMenuDomainLogic {
        public static IEnumerable<string> Get_Menus(IModelSchedulerPopupMenuItemEx popupMenuItem) {
            var enumValues = Enum.GetValues(typeof(SchedulerMenuItemId));
            return (enumValues.Cast<object>().Select(enumValue => enumValue.ToString())).ToList();
        }
    }

//    public class SchedulerMenuItemController : ViewController, IModelExtender {
//        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
////            extenders.Add<IModelSchedulerPopupMenu, IModelSchedulerPopupMenuItemEx>();
////            var interfaceBuilder = new InterfaceBuilder(extenders);
////            var builderData = new InterfaceBuilderData(typeof(SchedulerMenuItem)){Act = info =>{
////                if (info.Name == "Id")
////                    return false;
////                return info.DXFilter();
////            }
////            };
////            var assembly = interfaceBuilder.Build(new List<InterfaceBuilderData> { builderData }, GetPath(typeof(SchedulerMenuItem).Name));
////
////            interfaceBuilder.ExtendInteface<IModelSchedulerPopupMenuItem,SchedulerMenuItem>(assembly);
//        }
//    }

    public class SchedulerModelAdapterController : SchedulerModelAdapterControllerBase {
        public new SchedulerListEditor SchedulerListEditor => base.SchedulerListEditor as SchedulerListEditor;


//        protected override Type SchedulerControlType() {
//            return typeof (SchedulerControl);
//        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing += SchedulerControlOnPopupMenuShowing;
                ((ListView)View).CollectionSource.Criteria["ActiveViewFilter"] = CriteriaOperator.Parse("1=1");
            }
        }
        
//        protected override void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas) {
//            Type extenderType = typeof(SchedulerControl);
////            Assembly assembly = builder.Build(interfaceBuilderDatas, GetPath(extenderType.Name));
//
////            builder.ExtendInteface(typeof(IModelOptionsSchedulerEx), extenderType, assembly);
////            builder.ExtendInteface<IModelSchedulerPopupMenuItem, SchedulerPopupMenu>(assembly);
////            builder.ExtendInteface<IModelAppointmentLabel, AppointmentLabel>(assembly);
////            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageLabels>(assembly);
////            builder.ExtendInteface<IModelAppoitmentStatus, AppointmentStatus>(assembly);
////            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageStatuses>(assembly);
//        }

//        protected override Type[] BaseSchedulerControlTypes() {
//            return base.BaseSchedulerControlTypes().Concat(new[]{
//                typeof (SchedulerViewTypedRepositoryBase<SchedulerViewBase>), typeof (DayView),
//                typeof (TimelineView), typeof (WeekView)
//            }).ToArray();
//        }

        protected override IAppointmentStatusStorage Statuses() {
            if (SchedulerListEditor != null) return SchedulerListEditor.SchedulerControl.Storage.Appointments.Statuses;
            var schedulerStorage = SchedulerStorage();
            return schedulerStorage != null ? schedulerStorage.Appointments.Statuses : new AppointmentStatusCollection();
        }

        protected override IAppointmentLabelStorage Labels() {
            if (SchedulerListEditor != null) 
                return SchedulerListEditor.SchedulerControl.Storage.Appointments.Labels;
            var schedulerStorage = SchedulerStorage();
            return schedulerStorage != null ? schedulerStorage.Appointments.Labels : new AppointmentLabelCollection();
        }

        ISchedulerStorage SchedulerStorage() {
            var schedulerLabelPropertyEditor = ((DetailView) View).GetItems<SchedulerLabelPropertyEditor>().FirstOrDefault();
            return ((AppointmentLabelEdit) schedulerLabelPropertyEditor?.Control)?.Storage;
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
            return SchedulerListEditor.SchedulerControl.DataStorage;
        }

        void SchedulerControlOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e){
            SchedulerPopupMenu schedulerPopupMenu = e.Menu;
            SynchMenu(schedulerPopupMenu);
        }

        protected override object GetMenu(object popupMenu, IModelNode modelMenu){
            var menuId = ((IModelSchedulerPopupMenuItemEx)modelMenu).MenuId;
            var value = menuId.Change<SchedulerMenuItemId>();
            var items = ((SchedulerPopupMenu) popupMenu).Items;
            var menuItem = items.OfType<SchedulerPopupMenu>()
                .FirstOrDefault(item => item.GetPropertyValue("Id").Change<SchedulerMenuItemId>() == value);
            return menuItem;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing -= SchedulerControlOnPopupMenuShowing;
            }
        }

    }
}
