using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Web.SystemModule;

namespace Xpand.ExpressApp.Web.SystemModule {
    public class ViewEditModeController : ExpressApp.SystemModule.ViewEditModeController {
        protected override void UpdateViewEditModeState(DevExpress.ExpressApp.Editors.ViewEditMode viewEditMode) {
            Frame.GetController<WebDetailViewController>().EditAction.DoExecute();
        }
    }
}
