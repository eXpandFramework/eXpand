using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;

namespace Xpand.ExpressApp.Win {
    public class XpandWinWindow : WinWindow {
        public XpandWinWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly) {
        }
    }
}
