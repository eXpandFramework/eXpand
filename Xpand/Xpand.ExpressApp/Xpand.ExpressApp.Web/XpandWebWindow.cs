using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web {
    public class XpandWebWindow : WebWindow {
        public XpandWebWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly) {

            ClientScripts.Add("XpandHelper", XpandLayoutManager.GetXpandHelperScript());
        }

    }
}
