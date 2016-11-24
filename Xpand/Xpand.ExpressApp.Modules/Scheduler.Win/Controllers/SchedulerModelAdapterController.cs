using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Persistent.Base;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using Xpand.ExpressApp.Scheduler.Model;
using Xpand.Persistent.Base.ModelAdapter;
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

    public class SchedulerMenuItemAdapterController : ModelAdapterController, IModelExtender {
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelSchedulerPopupMenuItem, IModelSchedulerPopupMenuItemEx>();
            var interfaceBuilder = new InterfaceBuilder(extenders);
            var builderData = new InterfaceBuilderData(typeof(SchedulerMenuItem)){Act = info =>{
                if (info.Name == "Id")
                    return false;
                return info.DXFilter();
            }
            };
            var assembly = interfaceBuilder.Build(new List<InterfaceBuilderData> { builderData }, GetPath(typeof(SchedulerMenuItem).Name));

            interfaceBuilder.ExtendInteface<IModelSchedulerPopupMenuItem,SchedulerMenuItem>(assembly);
        }
    }

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
//            builder.ExtendInteface<IModelSchedulerPopupMenuItem, SchedulerPopupMenu>(assembly);
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

        void SchedulerControlOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e){
            var schedulerPopupMenu = e.Menu;
            SynchMenu(schedulerPopupMenu);
        }

        protected override object GetMenu(object popupMenu, IModelSchedulerPopupMenuItem modelMenu){
            var menuItemById = ((SchedulerPopupMenu) popupMenu).GetMenuItemById((SchedulerMenuItemId)
                ((IModelSchedulerPopupMenuItemEx)modelMenu).MenuId.Change(typeof(SchedulerMenuItemId)));
            return menuItemById;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing -= SchedulerControlOnPopupMenuShowing;
            }
        }

    }
}
