using DevExpress.ExpressApp.ConditionalAppearance;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.Controllers {
    public class UpdateVisibilityController : RefreshAppearanceController {
        protected override void RefreshAppearance() {
            if (Frame.GetController<AppearanceController>() == null)
                return;
            base.RefreshAppearance();
            if (View != null ) {
                var layoutManager = View.LayoutManager as XpandLayoutManager;
                if (layoutManager != null)
                    layoutManager.UpdateItemsVisibility();
            }
        }
    }
}