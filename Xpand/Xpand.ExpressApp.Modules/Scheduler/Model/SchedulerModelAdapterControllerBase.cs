using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.Utils.Controls;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Scheduler.Model {
    public abstract class SchedulerModelAdapterControllerBase : ModelAdapterController, IModelExtender {
        LinkToListViewController _linkToListViewController;

        public SchedulerListEditorBase SchedulerListEditor {
            get {
                var listView = View as ListView;
                return listView != null ? listView.Editor as SchedulerListEditorBase : null;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied += CollectionSourceOnCriteriaApplied;
                new SchedulerListEditorModelSynchronizer(SchedulerControl(), (IModelListViewOptionsScheduler)View.Model, Labels(), Statuses()).ApplyModel();
                SchedulerListEditor.ResourceDataSourceCreating += SchedulerListEditorOnResourceDataSourceCreating;
            }
            var detailView = View as DetailView;
            if (detailView != null && View.ObjectTypeInfo.Implements<IEvent>()) {
                _linkToListViewController = Frame.GetController<LinkToListViewController>();
                _linkToListViewController.LinkChanged+=LinkToListViewControllerOnLinkChanged;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied -= CollectionSourceOnCriteriaApplied;
                SchedulerListEditor.ResourceDataSourceCreating -= SchedulerListEditorOnResourceDataSourceCreating;
            }
            if (_linkToListViewController!=null)
                _linkToListViewController.LinkChanged-=LinkToListViewControllerOnLinkChanged;

        }

        void LinkToListViewControllerOnLinkChanged(object sender, EventArgs eventArgs) {
            var link = ((LinkToListViewController) sender).Link;
            if (SchedulerListEditor!=null && link != null && link.ListView != null) {
                new AppoitmentSynchronizer(Labels(), Statuses(), (IModelListViewOptionsScheduler)link.ListView.Model).ApplyModel();
            }	
        }

        protected abstract AppointmentStatusBaseCollection Statuses();

        protected abstract AppointmentLabelBaseCollection Labels();

        protected abstract IInnerSchedulerControlOwner SchedulerControl();

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
                    var storage = Storage();
                    storage.BeginUpdate();
                    var resources = Resources();
                    for (int i = 0; i < resources.Count; i++) {
                        resources[i].Visible = false;
                    }
                    foreach (var resourceId in Items().SelectMany(appointment => appointment.ResourceIds)) {
                        resources.GetResourceById(resourceId).Visible = true;
                    }
                    storage.EndUpdate();
                }
            }
        }

        protected abstract ResourceStorageBase Resources();

        protected abstract IEnumerable<Appointment> Items();
        protected abstract SchedulerStorageBase Storage();

        protected void SynchMenu(object menu) {
            var popupMenus = ((IModelListViewOptionsScheduler) View.Model).OptionsScheduler.PopupMenus;
            var schedulerMenuItemIds =popupMenus.Select(popupMenu =>new{ModelMenu = popupMenu,
                MenuId = (SchedulerMenuItemId) Enum.Parse(typeof (SchedulerMenuItemId), popupMenu.MenuId)});

            var menus = schedulerMenuItemIds.Select(arg => new{arg.ModelMenu, Menu = menu});
            foreach (var popupMenu in menus) {
                new SchedulerPopupMenuModelSynchronizer(popupMenu.Menu, popupMenu.ModelMenu).ApplyModel();
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewOptionsScheduler>();

            var builder = new InterfaceBuilder(extenders);
            var interfaceBuilderDatas = CreateBuilderData();
            
            Build(builder, interfaceBuilderDatas);
        }

        protected abstract void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas);


        protected virtual IEnumerable<InterfaceBuilderData> CreateBuilderData() {
            yield return new InterfaceBuilderData(SchedulerControlType()) {
                Act = info => {
                    info.RemoveInvalidTypeConverterAttributes("DevExpress.XtraScheduler.Design");
                    return info.DXFilter(BaseSchedulerControlTypes(), typeof(object));
                }
            };
        }

        protected abstract Type SchedulerControlType();

        protected virtual Type[] BaseSchedulerControlTypes() {
            return new[]{
                typeof (ResourceStorageBase), typeof (AppointmentMappingInfo), typeof (AppointmentStorageBase),
                typeof (BaseOptions), typeof (SchedulerStorageBase),typeof (ResourceMappingInfo)
            };
        }
    }

}
