using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.XtraBars;

namespace eXpand.ExpressApp.Win.SystemModule
{
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController{
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            var template = Frame.Template as NestedFrameTemplate;
            if (template != null) SetNestedToolbarVisibility(template, false);
        }

        void SetNestedToolbarVisibility(IBarManagerHolder template, bool visible){
            foreach (Bar bar in template.BarManager.Bars) {
                if (bar.BarName == "ListView Toolbar" || bar.BarName == "Main Toolbar"){
                    bar.Visible = visible;
                    break;
                }
            }
        }
    }
}
