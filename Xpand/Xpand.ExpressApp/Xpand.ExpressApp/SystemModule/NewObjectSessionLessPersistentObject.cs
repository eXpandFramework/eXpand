using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;
using System.Linq;

namespace Xpand.ExpressApp.SystemModule {

    public class NewObjectSessionLessPersistentObject : ViewController {
        protected override void OnFrameAssigned() {
            base.OnFrameAssigned();
            Frame.Disposing += FrameOnDisposing;
            var newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController.CollectDescendantTypes+=OnCollectDescendantTypes;
            newObjectViewController.CollectCreatableItemTypes+=NewObjectViewControllerOnCollectCreatableItemTypes;
        }

        void NewObjectViewControllerOnCollectCreatableItemTypes(object sender, CollectTypesEventArgs collectTypesEventArgs) {
            var modelCreatableItems = Application.Model as IModelApplicationCreatableItems;
            if (modelCreatableItems != null && modelCreatableItems.CreatableItems != null) {
                var creatableItems = modelCreatableItems.CreatableItems.Where(item => item.ModelClass.TypeInfo.FindAttribute<SessionLessPersistentAttribute>() != null).ToList();
                creatableItems.ForEach(creatableItem => collectTypesEventArgs.Types.Add(creatableItem.ModelClass.TypeInfo.Type));
            }
        }


        void OnCollectDescendantTypes(object sender, CollectTypesEventArgs collectTypesEventArgs) {
            var objectTypeInfo = ((NewObjectViewController)sender).View.ObjectTypeInfo;
            if (objectTypeInfo.FindAttribute<SessionLessPersistentAttribute>()!=null)
                collectTypesEventArgs.Types.Add(objectTypeInfo.Type);
        }


        void FrameOnDisposing(object sender, EventArgs eventArgs) {
            var newObjectViewController = Frame.GetController<NewObjectViewController>();
            newObjectViewController.CollectDescendantTypes-=OnCollectDescendantTypes;
            newObjectViewController.CollectCreatableItemTypes -= NewObjectViewControllerOnCollectCreatableItemTypes;
        }
    }

}
