using System.Collections.Generic;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.Web {
    public class XpandWebWindow : WebWindow {
        public XpandWebWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly) {
        }

        protected override void RegisterCommonScripts(Page page) {
            base.RegisterCommonScripts(page);
            page.ClientScript.RegisterClientScriptResource(typeof(XpandWebWindow), "Xpand.ExpressApp.Web.Layout.Splitter.js");
        }
    }
}
