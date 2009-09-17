using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.Scheduler;
using DevExpress.Persistent.Base.General;
using DevExpress.Xpo;
using Foxhound.ExpressApp.Scheduler.BaseObjects.Events;
using Foxhound.Persistent.Base.Interfaces;
using Foxhound.Persistent.BaseImpl;

namespace Foxhound.ExpressApp.Scheduler.Controllers{
    public enum SchedulerEventsView{
        StandardScheduler = 0,
        DaysOfWeek = 1,
        HoursOfDay = 2
    }

    public partial class SchedulerEventListViewController : ViewController{
        protected IHasResources masterObjectAsResourceContainer;
        protected ITypeInfo resourceTypeInfo;
        protected ITypeInfo masterObjectTypeInfo;
        protected string longerCaption;
        protected IDateRange masterObjectAsDateRange;
        protected SchedulerListEditorBase schedulerListEditorBase;

        public SchedulerEventListViewController(){
            InitializeComponent();
            RegisterActions(components);
        }

        protected virtual void CreateResourceTypeChoices(){
            selectResourceTypeAction.Items.Clear();
            if (masterObjectAsResourceContainer != null){
                selectResourceTypeAction.Items.AddRange(
                    masterObjectAsResourceContainer.ResourceHelper.ResourceTypes
                        .Select(info => new ChoiceActionItem(info.Name, info)).ToList());
            }
            else{
                selectResourceTypeAction.Items.AddRange(
                    (new ApplicationNodeWrapper(Application.Info).BOModel.Classes
                        .Where(boclass => boclass.ClassTypeInfo.Implements<IResource>())
                        .Select(boclass => new ChoiceActionItem(boclass.ClassTypeInfo.Name, boclass.ClassTypeInfo)))
                        .ToList()
                    );
            }
        }

        protected override void OnActivated(){
            base.OnActivated();
            //selectSchedulerEventViewAction.Populate();
            bool listEditorIsScheduler = ((ListView) View).Editor.GetType().IsSubclassOf(typeof (SchedulerListEditorBase));
            selectResourceTypeAction.Active["schedulerEditor"] = listEditorIsScheduler;

            if (listEditorIsScheduler){
                schedulerListEditorBase = ((SchedulerListEditorBase)((ListView)View).Editor);
                View.ControlsCreated += ListViewControlsCreatedHandler;
                schedulerListEditorBase.ResourceDataSourceCreated += ListEditorResourceDataSourceCreatedHandler;
                schedulerListEditorBase.Scheduler.StorageBase.ResourceCollectionLoaded += StorageBaseResourceCollectionLoadedHandler;
            }
        }

        protected virtual void StorageBaseResourceCollectionLoadedHandler(object sender, EventArgs e){
            //if (masterObjectAsResourceContainer != null){
                IEnumerable<ResourceImpl> resources = schedulerListEditorBase.Scheduler.Resources.Items
                    .Select(resource => new ResourceImpl{Caption = resource.Caption, Id = resource.Id});
                longerCaption = resources.SelectMany(resource => resource.CaptionLines).OrderByDescending(line => line.Length).FirstOrDefault();
            //}
        }

        private void ListViewControlsCreatedHandler(object sender, EventArgs e){
            var collectionSource = ((ListView) View).CollectionSource as PropertyCollectionSource;
            
            if (collectionSource != null){
                masterObjectTypeInfo = XafTypesInfo.Instance.FindTypeInfo(collectionSource.MasterObjectType);
                if (masterObjectTypeInfo.Implements<IResource>()){
                    resourceTypeInfo = masterObjectTypeInfo;
                }
                else if (masterObjectTypeInfo.Implements<IHasResources>()){
                    collectionSource.MasterObjectChanged += CollectionSourceMasterObjectChangedHandler;
                    resourceTypeInfo = null;
                    RetrieveMasterObject(collectionSource);
                }
            }
            else{
                Type resourceType = schedulerListEditorBase.GetResourceType();
                resourceTypeInfo = resourceType != null ? XafTypesInfo.Instance.FindTypeInfo(resourceType) : null;
            }

            if (masterObjectTypeInfo != null && masterObjectTypeInfo.Implements<IHasResources>()) {
                selectResourceTypeAction.Active["masterObjectResolution"] = true;
            }
            else if (masterObjectTypeInfo != null && masterObjectTypeInfo.Implements<IResource>()) {
                selectResourceTypeAction.Active["masterObjectResolution"] = false;
            } else {
                selectResourceTypeAction.Active["masterObjectResolution"] = true;
            }
            CreateResourceTypeChoices();
            SetSelecteResourceActionSelectedItem();
            CustomizeSchedulerControl();
        }

        private void RetrieveMasterObject(PropertyCollectionSource source){
            masterObjectAsResourceContainer = (IHasResources) source.MasterObject;
            if (masterObjectAsResourceContainer != null && resourceTypeInfo == null){
                resourceTypeInfo = masterObjectAsResourceContainer.ResourceHelper.ResourceTypes.FirstOrDefault();
            }
            if (masterObjectTypeInfo.Implements<IDateRange>()){
                masterObjectAsDateRange = (IDateRange) source.MasterObject;
            }
        }

        private void CollectionSourceMasterObjectChangedHandler(object sender, EventArgs e){
            RetrieveMasterObject((PropertyCollectionSource) sender);
        }

        private void SetSelecteResourceActionSelectedItem(){
            if (resourceTypeInfo != null){
                selectResourceTypeAction.SelectedItem = selectResourceTypeAction.Items
                                                            .Where(item => item.Data.Equals(resourceTypeInfo))
                                                            .SingleOrDefault();
                
                if (selectResourceTypeAction.SelectedItem == null){
                    resourceTypeInfo = null;
                }
            }
            UpdateListEditorResourceType();
        }

        protected virtual void UpdateListEditorResourceType(){
            if (resourceTypeInfo != null){
                ((ListView) View).Editor.Model.Node.SetAttribute("ResourceClassName", resourceTypeInfo.FullName);
            }
        }

        private void ListEditorResourceDataSourceCreatedHandler(object sender, ResourceDataSourceCreatedEventArgs e){
            var resources = (XPCollection) e.DataSource;
            var collectionSource = ((ListView) View).CollectionSource as PropertyCollectionSource;
            if (resources != null && resourceTypeInfo != null && collectionSource != null && collectionSource.MasterObject != null){
                if (masterObjectAsResourceContainer != null){
                    List<object> criteriaValues = masterObjectAsResourceContainer.ResourceHelper.Resources.Select(res => res.Id).ToList();
                    if (criteriaValues.Count == 0) {
                        criteriaValues.Add(Guid.Empty);
                    }
                    resources.Criteria = new InOperator("Oid", criteriaValues);
                } else {
                    resources.Criteria = new BinaryOperator("Oid", ((IResource)collectionSource.MasterObject).Id);
                }
            }
        }

        protected virtual void SelectResourceTypeActionExecuteHandler(object sender, SingleChoiceActionExecuteEventArgs e){
            resourceTypeInfo = (ITypeInfo) e.SelectedChoiceActionItem.Data;
            UpdateListEditorResourceType();
            View.Refresh();
        }

        protected virtual void CustomizeSchedulerControl(){
            if (masterObjectAsDateRange != null && masterObjectAsDateRange.FirstDay.HasValue) {
                schedulerListEditorBase.Scheduler.Start = masterObjectAsDateRange.FirstDay.Value;
            }
        }
    }
}