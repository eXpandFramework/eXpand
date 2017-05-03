using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Core;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule.Actions {
    public class DisableActionsLinqListViewController : ViewController<ListView> {
        private const string DefaultReason = "LinqListViewController is active";
        protected override void OnActivated() {
            base.OnActivated();
            bool flag = !(View.CollectionSource is LinqCollectionSource);
            Frame.GetController<ListViewProcessCurrentObjectController>(controller => controller.Active[DefaultReason] = flag);
            Frame.GetController<DeleteObjectsViewController>(controller => controller.Active[DefaultReason] = flag);
            Frame.GetController<NewObjectViewController>(controller => controller.Active[DefaultReason] = flag);
            Frame.GetController<FilterController>(controller => controller.Active[DefaultReason] = flag);
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<ListViewProcessCurrentObjectController>(controller => controller.Active.RemoveItem(DefaultReason));
            Frame.GetController<DeleteObjectsViewController>(controller => controller.Active.RemoveItem(DefaultReason));
            Frame.GetController<NewObjectViewController>(controller => controller.Active.RemoveItem(DefaultReason));
            Frame.GetController<FilterController>(controller => controller.Active.RemoveItem(DefaultReason));
            
        }
    }
}
