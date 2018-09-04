using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.CodeParser;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using Fasterflect;
using Xpand.ExpressApp.SystemModule;
using Xpand.Utils.Web;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class HideToolBarController : ExpressApp.SystemModule.HideToolBarController {
        private IEnumerable<ActionContainerHolder> _containerHolders;
        private string _visibility;
        private ISupportActionsToolbarVisibility _template;

        protected override void OnActivated(){
            base.OnActivated();
            if (WebApplicationStyleManager.IsNewStyle){
                Application.CustomizeTemplate+=ApplicationOnCustomizeTemplate;
            }
        }



        private void ApplicationOnCustomizeTemplate(object sender, CustomizeTemplateEventArgs e){
            if (e.Context == TemplateContext.NestedFrame) {
                _template = e.Template as ISupportActionsToolbarVisibility;
                if (_template != null) {
                    var modelViewHideViewToolBar = ((IModelClassHideViewToolBar)View.Model);
                    var hideToolBar = modelViewHideViewToolBar.HideToolBar.HasValue &&modelViewHideViewToolBar.HideToolBar.Value;
                    ((Control) _template).Init += (o, args) => {
                        var actionContainerHolders =new[]{_template.GetFieldValue("ToolBar"),_template.GetFieldValue("ObjectsCreation")}
                            .Cast<ActionContainerHolder>()
                            .Where(containerHolder => containerHolder!=null);
                        foreach (var container in actionContainerHolders) {
                            container.Style.Remove(HtmlTextWriterStyle.Visibility);
                            container.Style.Remove(HtmlTextWriterStyle.Display);
                            if (hideToolBar)
                                ApplyStyle(container);
                        }
                    };
                }
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (Frame.Template != null) {
                StyleToolBar();
            }
        }

        private void StyleToolBar(){
            _containerHolders = ((Control) Frame.Template).FindNestedControls<ActionContainerHolder>()
                .Where(holder => holder.ContainerStyle == ActionContainerStyle.ToolBar);
            foreach (var containerHolder in _containerHolders){
                var modelViewHideViewToolBar = ((IModelClassHideViewToolBar) View.Model);
                if (modelViewHideViewToolBar.HideToolBar.HasValue && modelViewHideViewToolBar.HideToolBar.Value) {
                    _visibility = containerHolder.Style[HtmlTextWriterStyle.Display];
                    ApplyStyle(containerHolder);
                }
            }
        }

        private static void ApplyStyle(ActionContainerHolder containerHolder){
            containerHolder.Style[HtmlTextWriterStyle.Visibility] = "Hidden";
            containerHolder.Style[HtmlTextWriterStyle.Display] = "None";
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