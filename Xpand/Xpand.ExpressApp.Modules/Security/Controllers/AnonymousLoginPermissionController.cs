using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Controllers {
    public abstract class AnonymousLogonWindowsController:WindowController {
        
        protected AnonymousLogonWindowsController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            if (!SecuritySystem.IsGranted(new IsAdministratorPermissionRequest())) {
                var isGranted = SecuritySystem.IsGranted(new AnonymousLoginOperationRequest(new AnonymousLoginPermission(Modifier.Allow)));
                Frame.GetController<LogoffController>().Active[typeof(AnonymousLoginPermission).Name] = !isGranted;
                Active[typeof (AnonymousLoginPermission).Name] = isGranted;
            }
            else {
                Active["IsAdmin"] = false;
            }
        }
    }
}
