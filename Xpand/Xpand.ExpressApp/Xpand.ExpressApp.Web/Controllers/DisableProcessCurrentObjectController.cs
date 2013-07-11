using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.Controllers {
    public class DisableProcessCurrentObjectController : ViewController<ListView> {
        private const string STR_DisableProcessCurrentObjectController = "DisableProcessCurrentObjectController";
        public DisableProcessCurrentObjectController() {
            TargetViewType = ViewType.ListView;
        }
        protected override void OnDeactivated() {
            if (IsMasterDetail) {
                Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active[STR_DisableProcessCurrentObjectController] = true;
                Frame.GetController<ListViewController>().EditAction.Active[STR_DisableProcessCurrentObjectController] = true;
            }
            base.OnDeactivated();
        }

        bool IsMasterDetail {
            get { return View.Model != null && View.Model.MasterDetailMode == MasterDetailMode.ListViewAndDetailView; }
        }
        protected override void OnActivated() {

            base.OnActivated();
            if (IsMasterDetail) {
                Frame.GetController<ListViewProcessCurrentObjectController>().ProcessCurrentObjectAction.Active[STR_DisableProcessCurrentObjectController] = false;
                Frame.GetController<ListViewController>().EditAction.Active[STR_DisableProcessCurrentObjectController] = false;
            }
        }
    }
}
