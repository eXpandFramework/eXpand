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
            var cssFiles = new[]{ResourceNames.CommonStyles, ResourceNames.CodeFormatter};
            string url;
            foreach (var cssFile in cssFiles) {
                url = clientScriptManager.GetWebResourceUrl(GetType(), cssFile);
                page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + url + "\" />"));    
            }
            
            url = clientScriptManager.GetWebResourceUrl(GetType(), ResourceNames.HighlightFocusedLayoutItem);
            page.Header.Controls.Add(new LiteralControl(@"<script language=""javascript"" src=""" + url + @"""></script>"));
        }
    }
}
