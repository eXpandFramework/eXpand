using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;

namespace Xpand.ExpressApp.Web {
    public class XpandWebWindow : WebWindow {
        public XpandWebWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly) {
        }
    }
}
