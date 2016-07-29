using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraBars;
using Xpand.ExpressApp.SystemModule;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var template = Frame.Template as IBarManagerHolder;
            if (template?.BarManager != null && ((IModelClassHideViewToolBar)View.Model).HideToolBar.HasValue) {
                var hideToolBar = ((IModelClassHideViewToolBar)View.Model).HideToolBar;
                SetToolbarVisibility(template, hideToolBar != null && !hideToolBar.Value);
            }
        }
        protected override void OnDeactivated() {
            base.OnDeactivated();
            var template = Frame.Template as IBarManagerHolder;
            if (template?.BarManager != null && ((IModelClassHideViewToolBar)View.Model).HideToolBar.HasValue) {
                var hideToolBar = ((IModelClassHideViewToolBar)View.Model).HideToolBar;
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

            var visibility = template as ISupportActionsToolbarVisibility;
            visibility?.SetVisible(visible);
        }
    }
}