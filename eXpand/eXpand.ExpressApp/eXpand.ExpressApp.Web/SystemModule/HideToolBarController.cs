using System.Web.UI;

namespace eXpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null) {
                Control control = ((Control) Frame.Template).FindControl("ToolBar");
                if (control != null) control.Visible = !View.Info.GetAttributeBoolValue(HideToolBarAttributeName);
            }
        }
    }
}