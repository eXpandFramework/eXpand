using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using eXpand.ExpressApp.SystemModule;
using eXpand.Utils.Web;
using System.Linq;

namespace eXpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null) {
                ActionContainerHolder containerHolder = ((Control) Frame.Template).FindNestedControls<ActionContainerHolder>("ToolBar").SingleOrDefault();
                if (containerHolder != null)
                    containerHolder.Visible = !((IModelViewHideViewToolBar) View.Model).HideToolBar;
            }
        }
    }
}