using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.SystemModule{
    public class NestedListViewTopReturnRecordsController : WindowController,IModelExtender{
        protected override void OnFrameAssigned(){
            base.OnFrameAssigned();
            if (Frame.Context == TemplateContext.ApplicationWindow){
                Application.CreateCustomPropertyCollectionSource+=ApplicationOnCreateCustomPropertyCollectionSource;
                Frame.Disposing+=FrameOnDisposing;
            }
        }
        class PropertyCollectionSource:DevExpress.ExpressApp.PropertyCollectionSource{
            public PropertyCollectionSource(IObjectSpace objectSpace, Type masterObjectType, object masterObject,
                IMemberInfo memberInfo) : base(objectSpace, masterObjectType, masterObject, memberInfo){
            }

            protected override void ApplyTopReturnedObjects() {
                if(originalCollection != null) {
                    ObjectSpace.SetTopReturnedObjectsCount(originalCollection, TopReturnedObjects);
                }
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            var frame = ((Frame) sender);
            frame.Disposing-=FrameOnDisposing;
            Application.CreateCustomPropertyCollectionSource-=ApplicationOnCreateCustomPropertyCollectionSource;
        }

        private void ApplicationOnCreateCustomPropertyCollectionSource(object sender, CreateCustomPropertyCollectionSourceEventArgs e){
            var modelListVIew = ((IModelListViewNonPersistentObjectSpace) Application.Model.Views[e.ListViewID]);
            if (modelListVIew != null && (modelListVIew.TopReturnedObjects>0)){
                e.PropertyCollectionSource=new PropertyCollectionSource(e.ObjectSpace,e.MasterObjectType, e.MasterObject, e.MemberInfo);
             }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewNonPersistentObjectSpace>();
        }
    }
}