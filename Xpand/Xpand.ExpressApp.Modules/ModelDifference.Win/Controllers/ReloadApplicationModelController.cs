using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.ModelDifference.Win.Controllers {
    public class ReloadApplicationModelController : ModelDifference.Controllers.ReloadApplicationModelController {
        protected override void ReplaceLayer() {
            base.ReplaceLayer();
            var showViewStrategyBase = (WinShowViewStrategyBase)Application.ShowViewStrategy;
            showViewStrategyBase.CloseAllWindows();
            showViewStrategyBase.ShowStartupWindow();
        }
    }
}
