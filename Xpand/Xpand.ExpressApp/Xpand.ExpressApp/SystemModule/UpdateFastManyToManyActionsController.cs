using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.SystemModule {
    public class UpdateFastManyToManyActionsController : ViewController {
        public UpdateFastManyToManyActionsController() {
            TargetViewType = ViewType.ListView;
            TargetViewNesting = Nesting.Nested;
            TargetObjectType = typeof(IFastManyToMany);
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (!View.Id.EndsWith("LookupListView")) {
                Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("IFastManyToMany", false);
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("IFastManyToMany", false);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (!View.Id.EndsWith("LookupListView")) {
                Frame.GetController<NewObjectViewController>().NewObjectAction.Active.SetItemValue("IFastManyToMany", true);
                Frame.GetController<DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("IFastManyToMany", true);
            }
        }
    }
}
