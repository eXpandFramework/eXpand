using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.Security.AuthenticationProviders;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.Security.Controllers {
    public abstract class AnonymousLogonController:WindowController {
        
        protected AnonymousLogonController() {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnActivated() {
            base.OnActivated();
            var authentication = ((IModelOptionsAuthentication)Application.Model.Options).Athentication.AnonymousAuthentication;
            if (authentication.Enabled){
                var isGranted =SecuritySystem.IsGranted(new AnonymousLoginOperationRequest(new AnonymousLoginPermission(Modifier.Allow))) &&
                    !SecuritySystem.IsGranted(new AdministrativePermissionRequest());
                var logoffAction = Frame.GetController<LogoffController>().LogoffAction;
            
                logoffAction.Caption = isGranted? authentication.LoginActionCaption : logoffAction.Model.Caption;
                logoffAction.ToolTip = isGranted ? authentication.LoginActionTooltip : logoffAction.Model.ToolTip;
            }
        }
    }
}
