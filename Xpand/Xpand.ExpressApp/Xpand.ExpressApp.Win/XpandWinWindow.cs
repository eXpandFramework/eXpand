using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win {
    public class XpandWinWindow : WinWindow {
        public XpandWinWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers, bool isMain, bool activateControllersImmediatelly)
            : base(application, context, controllers, isMain, activateControllersImmediatelly) {
        }
        protected override void OnViewChanged(Frame sourceFrame) {
            base.OnViewChanged(sourceFrame);
            var xafApplication = Application as IAfterViewShown;
            if ((xafApplication != null) && (View != null)) {
                xafApplication.OnAfterViewShown(this, sourceFrame);
            }
        }
    }
}
