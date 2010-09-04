using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using Xpand.ExpressApp.SystemModule;
using System.Linq;
using Xpand.Utils.Web;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null && ((IModelViewHideViewToolBar)View.Model).HideToolBar.HasValue)
            {
                ActionContainerHolder containerHolder = ((Control) Frame.Template).FindNestedControls<ActionContainerHolder>("ToolBar").SingleOrDefault();
                if (containerHolder != null)
                    containerHolder.Visible = !((IModelViewHideViewToolBar) View.Model).HideToolBar.Value;
            }
        }
    }
}