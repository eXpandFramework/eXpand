using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Controllers {
    public class ShowNavigationItemController : DevExpress.ExpressApp.SystemModule.ShowNavigationItemController {
        protected override bool SyncItemsWithRequestSecurity(ChoiceActionItemCollection items){
            var syncItemsWithRequestSecurity = base.SyncItemsWithRequestSecurity(items);
            foreach (var item in items.Where(item => item.Active[SecurityVisibleKey]&&item.Enabled[SecurityVisibleKey])){
                if (SecuritySystem.Instance is IRequestSecurity && !SecuritySystem.IsGranted(new NavigationItemPermissionRequest(item.Id))){
                    item.Active[SecurityVisibleKey] = false;
                    item.Enabled[SecurityVisibleKey] = false;
                }
            }
            return syncItemsWithRequestSecurity;
        }
    }
}
