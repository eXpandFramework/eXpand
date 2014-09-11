using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General.Model;

namespace Xpand.Persistent.Base.General.Controllers {
    public interface IModelClassModifyObjectSpace{
        [Category(AttributeCategoryNameProvider.Xpand)]
        bool ModifyObjectSpace { get; set; }
    }

    [ModelInterfaceImplementor(typeof(IModelClassModifyObjectSpace),"ModelClass")]
    public interface IModelObjectViewModifyObjectSpace:IModelClassModifyObjectSpace{
         
    }
    public class ModifyObjectSpaceController:ViewController<ObjectView> ,IModelExtender{
        protected override void OnActivated(){
            base.OnActivated();
            if (((IModelObjectViewModifyObjectSpace) View.Model).ModifyObjectSpace){
                if (ObjectSpace.IsNewObject(View.CurrentObject)||!View.ObjectTypeInfo.IsPersistent)
                    ObjectSpace.SetIsModified(true);
                ObjectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            ObjectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
        }

        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs){
            ObjectSpace.SetIsModified(true);
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders){
            extenders.Add<IModelClass,IModelClassModifyObjectSpace>();
            extenders.Add<IModelObjectView,IModelObjectViewModifyObjectSpace>();
        }
    }
}
