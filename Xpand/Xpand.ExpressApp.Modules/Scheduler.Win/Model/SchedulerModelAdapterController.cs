using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.Scheduler.Win;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Win.Model {
    public class SchedulerModelAdapterController : ModelAdapterController, IModelExtender {

        public SchedulerListEditor SchedulerListEditor {
            get {
                var listView = View as ListView;
                return listView != null ? listView.Editor as SchedulerListEditor : null;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied += CollectionSourceOnCriteriaApplied;
                new SchedulerListEditorModelSynchronizer(SchedulerListEditor).ApplyModel();
                SchedulerListEditor.SchedulerControl.PopupMenuShowing += SchedulerControlOnPopupMenuShowing;
                SchedulerListEditor.ResourceDataSourceCreating += SchedulerListEditorOnResourceDataSourceCreating;
            }
        }

        void SchedulerListEditorOnResourceDataSourceCreating(object sender, ResourceDataSourceCreatingEventArgs e) {
            var resourceListView = ((IModelListViewOptionsScheduler)View.Model).ResourceListView;
            if (resourceListView != null) {
                var collectionSourceBase = Application.CreateCollectionSource(Application.CreateObjectSpace(e.ResourceType), e.ResourceType, resourceListView.Id, false, CollectionSourceMode.Proxy);
                Application.CreateListView(resourceListView.Id, collectionSourceBase, true);
                e.DataSource = collectionSourceBase.Collection;
                e.Handled = true;
            }
        }

        void CollectionSourceOnCriteriaApplied(object sender, EventArgs eventArgs) {
            if (((ListView)View).CollectionSource.Criteria.ContainsKey("ActiveViewFilter")) {
                var modelListViewOptionsScheduler = ((IModelListViewOptionsScheduler)View.Model);
                if (modelListViewOptionsScheduler.ResourcesOnlyWithAppoitments) {
                    SchedulerListEditor.SchedulerControl.Storage.BeginUpdate();
                    var schedulerStorage = SchedulerListEditor.SchedulerControl.Storage;
                    for (int i = 0; i < schedulerStorage.Resources.Count; i++) {
                        schedulerStorage.Resources[i].Visible = false;
                    }
                    foreach (var resourceId in schedulerStorage.Appointments.Items.SelectMany(appointment => appointment.ResourceIds)) {
                        schedulerStorage.Resources.GetResourceById(resourceId).Visible = true;
                    }
                    SchedulerListEditor.SchedulerControl.Storage.EndUpdate();
                }
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                SchedulerListEditor.SchedulerControl.PopupMenuShowing -= SchedulerControlOnPopupMenuShowing;
            }
        }

        void SchedulerControlOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs popupMenuShowingEventArgs) {
            var popupMenus = ((IModelListViewOptionsScheduler)View.Model).OptionsScheduler.PopupMenus;
            var schedulerMenuItemIds = popupMenus.Select(popupMenu => new { ModelMenu = popupMenu, MenuId = (SchedulerMenuItemId)Enum.Parse(typeof(SchedulerMenuItemId), popupMenu.MenuId) });
            var menus = schedulerMenuItemIds.Select(arg => new { arg.ModelMenu, Menu = popupMenuShowingEventArgs.Menu.GetPopupMenuById(arg.MenuId) });
            foreach (var popupMenu in menus) {
                new SchedulerPopupMenuModelSynchronizer(popupMenu.Menu, popupMenu.ModelMenu).ApplyModel();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsScheduler>();

            var builder = new InterfaceBuilder(extenders);
            var interfaceBuilderDatas = CreateBuilderData();
            var assembly = builder.Build(interfaceBuilderDatas, GetPath(typeof(SchedulerControl).Name));

            builder.ExtendInteface<IModelOptionsSchedulerEx, SchedulerControl>(assembly);
            builder.ExtendInteface<IModelSchedulerPopupMenu, SchedulerPopupMenu>(assembly);
            builder.ExtendInteface<IModelAppoitmentLabel, AppointmentLabel>(assembly);
            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageLabels>(assembly);
            builder.ExtendInteface<IModelAppoitmentStatus, AppointmentStatus>(assembly);
            builder.ExtendInteface<AppointmentStorage, IAppoitmentStorageStatuses>(assembly);
        }

        IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(typeof(SchedulerControl)) {
                Act = info => {
                    info.RemoveInvalidTypeConverterAttributes("DevExpress.XtraScheduler.Design");
                    return info.DXFilter(BaseSchudulerControlTypes(), typeof(object));
                }
            };
            yield return new InterfaceBuilderData(typeof(SchedulerPopupMenu)) {
                Act = info => info.Name != "Id" && info.DXFilter(typeof(DXMenuItem))
            };
            yield return new InterfaceBuilderData(typeof(AppointmentLabel)) {
                Act = info => info.DXFilter(typeof(UserInterfaceObject))
            };
            yield return new InterfaceBuilderData(typeof(AppointmentStatus)) {
                Act = info => info.DXFilter(typeof(UserInterfaceObject))
            };
        }

        Type[] BaseSchudulerControlTypes() {
            return new[]{
                typeof (SchedulerResourceHeaderOptions), typeof (SchedulerStorageBase),
                typeof (AppointmentStorage), typeof (ResourceStorage), typeof (AppointmentDependencyStorage),
                typeof (SchedulerViewTypedRepositoryBase<SchedulerViewBase>), typeof (DayView),
                typeof (TimelineView), typeof (WeekView), typeof (ResourceMappingInfo),
                typeof (ResourceStorageBase), typeof (AppointmentMappingInfo), typeof (AppointmentStorageBase)
            };
        }
    }
}
