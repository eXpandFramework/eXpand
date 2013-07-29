using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.SystemModule.MasterDetail {
    public class RegisterScriptsController:WindowController {
        protected override void OnActivated() {
            base.OnActivated();
            ((WebWindow)Frame).RegisterClientScript("XpandHelper", XpandLayoutManager.GetXpandHelperScript(),true);
        }
    }
}
