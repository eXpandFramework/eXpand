using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web;
using Xpand.ExpressApp.Web.Layout;

namespace Xpand.ExpressApp.Web {
    public class XpandPopupWindow : PopupWindow {
        public XpandPopupWindow(XafApplication application, TemplateContext context, ICollection<Controller> controllers)
            : base(application, context, controllers) {
        }

        
    }
}
