using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;

namespace eXpand.ExpressApp.SystemModule
{
    public abstract class ShowNonPersistentObjectDetailViewFromNavigationControllerBase<NonPersistentObjectType> : BaseViewController 
    {
        private const string DefaultReason = "ShowNonPersistentObjectDetailViewFromNavigationControllerBase is active";

        protected ShowNonPersistentObjectDetailViewFromNavigationControllerBase()
        {
            TargetObjectType = typeof(NonPersistentObjectType);
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            Frame.GetController<ShowNavigationItemController>().CustomShowNavigationItem += OnCustomShowNavigationItem;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateControllersState(false);
        }
        protected override void OnDeactivating()
        {
            base.OnDeactivating();
            UpdateControllersState(true);
        }
        protected virtual void UpdateControllersState(bool flag)
        {
            Frame.GetController<DetailViewController>().Active[DefaultReason] = flag;
            Frame.GetController<DeleteObjectsViewController>().Active[DefaultReason] = flag;
            Frame.GetController<NewObjectViewController>().Active[DefaultReason] = flag;
            Frame.GetController<DevExpress.ExpressApp.SystemModule.FilterController>().Active[DefaultReason] = flag;
            Frame.GetController<ViewNavigationController>().Active[DefaultReason] = flag;
            Frame.GetController<RecordsNavigationController>().Active[DefaultReason] = flag;
        }
        void OnCustomShowNavigationItem(object sender, CustomShowNavigationItemEventArgs e)
        {
            if (e.ActionArguments.SelectedChoiceActionItem.Info.GetAttributeValue("ViewID") == Application.FindListViewId(typeof(NonPersistentObjectType)))
            {
                ObjectSpace os = ObjectSpaceInMemory.CreateNew();
                NonPersistentObjectType obj = CreateNonPersistentObject(os);
                CustomizeNonPersistentObject(obj);
                DetailView dv = Application.CreateDetailView(os, obj, true);
                e.ActionArguments.ShowViewParameters.CreatedView = dv;
                CustomizeShowViewParameters(e.ActionArguments.ShowViewParameters);
                e.Handled = true;
            }
        }


        protected virtual NonPersistentObjectType CreateNonPersistentObject(ObjectSpace os)
        {
            NonPersistentObjectType obj = default(NonPersistentObjectType);
            var type = typeof(NonPersistentObjectType);
            ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(type);
            var dcAttribute = typeInfo.FindAttribute<DomainComponentAttribute>(false);
            var npAttribute = typeInfo.FindAttribute<NonPersistentAttribute>(false);
            if (typeof (PersistentBase).IsAssignableFrom(type)) {
                if (npAttribute != null) {
                    obj = (NonPersistentObjectType) os.CreateObject(type);
                }
            }
            else {
                if (dcAttribute != null || npAttribute != null) {
                    obj = (NonPersistentObjectType) Activator.CreateInstance(type);
                }
            }
            if (Equals(obj, default(NonPersistentObjectType))) {
                throw new InvalidOperationException("Cannot create an object of a non-persistent type.");
            }
            return obj;
        }

        protected virtual void CustomizeShowViewParameters(ShowViewParameters parameters)
        {
            parameters.Context = TemplateContext.ApplicationWindow;
            parameters.TargetWindow = TargetWindow.Current;
            ((DetailView)parameters.CreatedView).ViewEditMode = ViewEditMode.Edit;
        }
        protected virtual void CustomizeNonPersistentObject(NonPersistentObjectType obj) { }
    }
}