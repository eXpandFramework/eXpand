using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using Xpand.ExpressApp.SystemModule;
using Xpand.Utils.Web;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        private IEnumerable<ActionContainerHolder> _containerHolders;
        private string _visibility;

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null){
                _containerHolders = ((Control)Frame.Template).FindNestedControls<ActionContainerHolder>().Where(holder => holder.ContainerStyle==ActionContainerStyle.ToolBar);
                foreach (var containerHolder in _containerHolders){
                    var modelViewHideViewToolBar = ((IModelViewHideViewToolBar)View.Model);
                    if (_containerHolders != null && modelViewHideViewToolBar.HideToolBar.HasValue && modelViewHideViewToolBar.HideToolBar.Value) {
                        _visibility = containerHolder.Style[HtmlTextWriterStyle.Display];
                        containerHolder.Style[HtmlTextWriterStyle.Visibility] = "Hidden";
                    }    
                }
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_containerHolders != null){
                foreach (var containerHolder in _containerHolders){
                    containerHolder.Style[HtmlTextWriterStyle.Display] = _visibility;    
                }
            }
        }
    }
}