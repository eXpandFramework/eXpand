using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    public class StateMachineTransitionRequestProcessor : PermissionRequestProcessorBase<StateMachineTransitionOperationRequest> {
        protected override bool IsRequestFit(StateMachineTransitionOperationRequest permissionRequest, OperationPermissionBase permission, IRequestSecurityStrategy securityInstance) {
            if (permission is StateMachineTransitionPermission) {
                return permissionRequest.Modifier == ((StateMachineTransitionPermission)permission).Modifier &&
                       permissionRequest.StateCaption == ((StateMachineTransitionPermission)permission).StateCaption &&
                       permissionRequest.StateMachineName == ((StateMachineTransitionPermission)permission).StateMachineName;
            }
            return false;
        }

    }
}