using System;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.ExpressApp.SystemModule{
    [ModelAbstractClass]
    public interface IModelListViewNonPersistentObjectSpace:IModelListView{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool UseNonPersistentObjectSpaceWhenNested{ get; set; }
    }
    public class NestedListViewNonPersistentObjectSpaceController : WindowController,IModelExtender{
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

            protected override object CreateCollection(){
                return objectSpace.CreateCollection(ObjectTypeInfo.Type,null,null);
            }
        }

        private void FrameOnDisposing(object sender, EventArgs eventArgs){
            var frame = ((Frame) sender);
            frame.Disposing-=FrameOnDisposing;
            Application.CreateCustomPropertyCollectionSource-=ApplicationOnCreateCustomPropertyCollectionSource;
        }

        private void ApplicationOnCreateCustomPropertyCollectionSource(object sender, CreateCustomPropertyCollectionSourceEventArgs e){
            var modelListVIew = ((IModelListViewNonPersistentObjectSpace) Application.Model.Views[e.ListViewID]);
            if (modelListVIew != null && (!modelListVIew.ModelClass.TypeInfo.IsPersistent&&modelListVIew.UseNonPersistentObjectSpaceWhenNested)){
                var objectSpace = Application.CreateObjectSpace(e.MemberInfo.ListElementType);
                e.PropertyCollectionSource=new PropertyCollectionSource(objectSpace,e.MasterObjectType, e.MasterObject, e.MemberInfo);
            }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelListView,IModelListViewNonPersistentObjectSpace>();
        }
    }
}