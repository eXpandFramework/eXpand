using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using Xpand.ExpressApp.SystemModule;
using Xpand.Utils.Web;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        private IEnumerable<ActionContainerHolder> _containerHolders;
        private string _visibility;
        protected override void OnActivated(){
            base.OnActivated();
            if (WebApplicationStyleManager.IsNewStyle){
                Application.CustomizeTemplate+=ApplicationOnCustomizeTemplate;
            }
        }


        private void ApplicationOnCustomizeTemplate(object sender, CustomizeTemplateEventArgs e){
            if (e.Context == TemplateContext.NestedFrame) {
                var modelViewHideViewToolBar = ((IModelClassHideViewToolBar)View.Model);
                if (modelViewHideViewToolBar.HideToolBar.HasValue && modelViewHideViewToolBar.HideToolBar.Value){
                    var template = e.Template as ISupportActionsToolbarVisibility;
                    template?.SetVisible(false);
                }
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null){
                _containerHolders = ((Control)Frame.Template).FindNestedControls<ActionContainerHolder>().Where(holder => holder.ContainerStyle==ActionContainerStyle.ToolBar);
                foreach (var containerHolder in _containerHolders){
                    var modelViewHideViewToolBar = ((IModelClassHideViewToolBar)View.Model);
                    if (_containerHolders != null && modelViewHideViewToolBar.HideToolBar.HasValue && modelViewHideViewToolBar.HideToolBar.Value) {
                        _visibility = containerHolder.Style[HtmlTextWriterStyle.Display];
                        containerHolder.Style[HtmlTextWriterStyle.Visibility] = "Hidden";
                        containerHolder.Style[HtmlTextWriterStyle.Display] = "None";
                    }    
                }
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            Application.CustomizeTemplate -= ApplicationOnCustomizeTemplate;
            if (_containerHolders != null){
                foreach (var containerHolder in _containerHolders){
                    containerHolder.Style[HtmlTextWriterStyle.Display] = _visibility;    
                }
                _containerHolders = null;
            }
        }
    }
}