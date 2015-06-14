using System;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.SystemModule.MasterDetail{
    public class RegisterCallbackPanelScriptsController : WindowController{
        protected override void OnActivated(){
            base.OnActivated();
            var window = Window as WebWindow;
            if (window != null){
                window.PagePreRender += window_PagePreRender;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            var window = Window as WebWindow;
            if (window != null){
                window.PagePreRender -= window_PagePreRender;
            }
        }

        private void window_PagePreRender(object sender, EventArgs e){
            var window = (WebWindow) sender;
            var page = (Page) window.Template;
            RegisterPanelScripts(page);
        }

        private static void RegisterPanelScripts(Page page){
            if (!page.Controls.OfType<ASPxCallbackPanel>().Any())
                page.Controls.Add(new ASPxCallbackPanel());
        }
    }
}