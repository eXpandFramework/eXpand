using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraBars;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var template = Frame.Template as IBarManagerHolder;
            if (template != null && template.BarManager != null && ((IModelViewHideViewToolBar)View.Model).HideToolBar.HasValue) {
                var hideToolBar = ((IModelViewHideViewToolBar)View.Model).HideToolBar;
                SetToolbarVisibility(template, hideToolBar != null && !hideToolBar.Value);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            var template = Frame.Template as IBarManagerHolder;
            if (template != null && template.BarManager != null && ((IModelViewHideViewToolBar)View.Model).HideToolBar.HasValue) {
                var hideToolBar = ((IModelViewHideViewToolBar)View.Model).HideToolBar;
                SetToolbarVisibility(template, hideToolBar != null && hideToolBar.Value);
            }
        }
        void SetToolbarVisibility(IBarManagerHolder template, bool visible) {
            foreach (Bar bar in template.BarManager.Bars) {
                if (bar.BarName == "ListView Toolbar" || bar.BarName == "Main Toolbar") {
                    bar.Visible = visible;
                    break;
                }
            }

            if (template is ISupportActionsToolbarVisibility)
                ((ISupportActionsToolbarVisibility)template).SetVisible(visible);
        }
    }
}