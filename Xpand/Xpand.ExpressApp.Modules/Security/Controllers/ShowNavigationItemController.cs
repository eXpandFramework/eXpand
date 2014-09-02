﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Controllers {
    public class ShowNavigationItemController : DevExpress.ExpressApp.SystemModule.ShowNavigationItemController {
        protected override void SynchItemWithSecurity(DevExpress.ExpressApp.Actions.ChoiceActionItem item) {
            if (!SecuritySystem.IsGranted(new NavigationItemPermissionRequest(item.Id)) && SecuritySystem.Instance is IRequestSecurity) {
                item.Active[SecurityVisibleKey] = false;
            }
            else {
                base.SynchItemWithSecurity(item);
            }
        }
    }
}
