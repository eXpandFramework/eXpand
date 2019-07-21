using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
using Fasterflect;
using Xpand.Persistent.Base.General;
using Xpand.XAF.Modules.ModelMapper.Configuration;
using Xpand.XAF.Modules.ModelMapper.Services;
using Xpand.XAF.Modules.ModelMapper.Services.Predefined;

namespace Xpand.ExpressApp.Scheduler.Model {
    public abstract class SchedulerModelAdapterControllerBase : ViewController, IModelExtender {

        public SchedulerListEditorBase SchedulerListEditor {
            get {
                var listView = View as ListView;
                return listView?.Editor as SchedulerListEditorBase;
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied += CollectionSourceOnCriteriaApplied;
                SchedulerListEditor.ResourceDataSourceCreating += SchedulerListEditorOnResourceDataSourceCreating;
            }

            if (View is DetailView && View.ObjectTypeInfo.Implements<IEvent>()) {
                Frame.GetController<LinkToListViewController>(controller => controller.LinkChanged += LinkToListViewControllerOnLinkChanged);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (SchedulerListEditor != null) {
                ((ListView)View).CollectionSource.CriteriaApplied -= CollectionSourceOnCriteriaApplied;
                SchedulerListEditor.ResourceDataSourceCreating -= SchedulerListEditorOnResourceDataSourceCreating;
            }
            Frame.GetController<LinkToListViewController>(controller => controller.LinkChanged -= LinkToListViewControllerOnLinkChanged);

        }

        void LinkToListViewControllerOnLinkChanged(object sender, EventArgs eventArgs) {
            void Bind(Link link1, string modelNodeId, IEnumerable storage){
                var modelNodes = ((IEnumerable<IModelNode>) link1.ListView.Model.GetNode(PredefinedMap.SchedulerControl)
                    .GetNode("Appointments").GetNode(modelNodeId)).ToArray();
                foreach (var instance in storage){
                    var modelNode = modelNodes.First(node => node.Id() == (string) instance.GetPropertyValue("Id"));
                    modelNode.BindTo(instance);
                }
            }

            var link = ((LinkToListViewController) sender).Link;
            if (SchedulerListEditor!=null && link?.ListView != null) {
                Bind(link, "Labels", Labels());
                Bind(link, "Statuses", Statuses());
//                new AppoitmentSynchronizer(Labels(), Statuses(), (IModelListViewOptionsScheduler)link.ListView.Model).ApplyModel();
            }	
        }

        protected abstract IAppointmentStatusStorage Statuses();

        protected abstract IAppointmentLabelStorage Labels();

        protected abstract IInnerSchedulerControlOwner SchedulerControl();
        
        void SchedulerListEditorOnResourceDataSourceCreating(object sender, ResourceDataSourceCreatingEventArgs e) {
            var resourceListView = ((IModelListViewSchedulerEx) View.Model.GetNode(PredefinedMap.SchedulerControl.ToString())).ResourceListView;
            if (resourceListView != null) {
                var collectionSourceBase = Application.CreateCollectionSource(Application.CreateObjectSpace(e.ResourceType), e.ResourceType, resourceListView.Id, false, CollectionSourceMode.Proxy);
                Application.CreateListView(resourceListView.Id, collectionSourceBase, true);
                e.DataSource = collectionSourceBase.Collection;
                e.Handled = true;
            }
        }
        
        void CollectionSourceOnCriteriaApplied(object sender, EventArgs eventArgs) {
            if (((ListView)View).CollectionSource.Criteria.ContainsKey("ActiveViewFilter")) {
                var modelListViewOptionsScheduler = ((IModelListViewSchedulerEx)View.Model);
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
        protected abstract ISchedulerStorageBase Storage();
        
        protected void SynchMenu(object menu) {
            var popupMenus = ((IEnumerable<IModelNode>) View.Model.GetNode(PredefinedMap.SchedulerControl.ToString()).GetNode(SchedulerControlService.PopupMenusMoelPropertyName));
            foreach (var popupMenu in popupMenus){
                var component = GetMenu(menu,popupMenu);
                if (component != null) {
                    popupMenu.BindTo(component);
                }
            }
        }

        protected virtual object GetMenu(object popupMenu, IModelNode modelMenu){
            return popupMenu;
        }
        
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelListView, IModelListViewSchedulerEx>();

//            var builder = new InterfaceBuilder(extenders);
//            var interfaceBuilderDatas = CreateBuilderData();
            
//            Build(builder, interfaceBuilderDatas);
        }

//        protected abstract void Build(InterfaceBuilder builder, IEnumerable<InterfaceBuilderData> interfaceBuilderDatas);


//        protected virtual IEnumerable<InterfaceBuilderData> CreateBuilderData() {
//            yield return new InterfaceBuilderData(SchedulerControlType()) {
//                Act = info =>{
//                    if (info.Name == "Item"&&!info.PropertyType.BehaveLikeValueType())
//                        return false;
//                    if (info.Name == "DataStorage") {
//                        info.SetName("Storage");
//                        info.SetPropertyType(typeof(SchedulerStorage));
//                    }
//                    info.RemoveInvalidTypeConverterAttributes("DevExpress.XtraScheduler.Design");
//                    return info.DXFilter(BaseSchedulerControlTypes(), typeof(object));
//                }
//            };
//        }

//        protected abstract Type SchedulerControlType();

//        protected virtual Type[] BaseSchedulerControlTypes() {
//            return new[]{
//                typeof (ResourceStorageBase), typeof (AppointmentMappingInfo), typeof (AppointmentStorageBase),
//                typeof (BaseOptions), typeof (SchedulerStorageBase),typeof (ResourceMappingInfo)
//            };
//        }
    }

}
