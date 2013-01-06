using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.ConditionalAppearance;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.Controllers {
    public class UpdateVisibilityController : RefreshAppearanceController {

        protected override void RefreshAppearance() {
            base.RefreshAppearance();
            if (View != null) {
                XpandLayoutManager layoutManager = View.LayoutManager as XpandLayoutManager;
                if (layoutManager != null)
                    layoutManager.UpdateItemsVisibility();
            }
        }
    }
}
