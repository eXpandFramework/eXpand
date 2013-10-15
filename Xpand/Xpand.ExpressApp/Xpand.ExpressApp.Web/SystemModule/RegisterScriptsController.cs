using System;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class RegisterScriptsController:WindowController {
        protected override void OnActivated() {
            base.OnActivated();
            var webWindow = ((WebWindow) Frame);
            webWindow.RegisterClientScript("XpandHelper", XpandLayoutManager.GetXpandHelperScript(),true);
            webWindow.PagePreRender+=WebWindowOnPagePreRender;
            
        }

        protected virtual void WebWindowOnPagePreRender(object sender, EventArgs eventArgs) {
            var page = WebWindow.CurrentRequestPage;
            var clientScriptManager = page.ClientScript;
            var url = clientScriptManager.GetWebResourceUrl(GetType(), ResourceNames.CommonStyles);
            page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + url + "\" />"));
        }
    }
}
