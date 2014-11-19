using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.SystemModule.MasterDetail {
    public class DisableProcessCurrentObjectController : ViewController<ListView> {
        private const string StrDisableProcessCurrentObjectController = "DisableProcessCurrentObjectController";
        public DisableProcessCurrentObjectController() {
            TargetViewType = ViewType.ListView;
        }
        protected override void OnDeactivated() {
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active[StrDisableProcessCurrentObjectController] = IsMasterDetail;
            base.OnDeactivated();
        }

        bool IsMasterDetail {
            get { return View.Model != null && View.Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView&&View.LayoutManager is XpandLayoutManager; }
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active[StrDisableProcessCurrentObjectController] = !IsMasterDetail;
        }
    }
}
