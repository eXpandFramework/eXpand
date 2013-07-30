using DevExpress.ExpressApp.Security;
using System.Linq;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    public class StateMachineTransitionRequestProcessor : PermissionRequestProcessorBase<StateMachineTransitionOperationRequest> {
        readonly IPermissionDictionary _permissions;

        public StateMachineTransitionRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }


        public override bool IsGranted(StateMachineTransitionOperationRequest permissionRequest) {
            var permissions = _permissions.GetPermissions<StateMachineTransitionPermission>();
            var stateMachineTransitionPermissions = permissions.Where(permission => TransitionMatch(permission, permissionRequest));
            if (!stateMachineTransitionPermissions.Any())
                return true;
            return stateMachineTransitionPermissions.Any(permission => permission.Hide==permissionRequest.Hide);
        }


        bool TransitionMatch(StateMachineTransitionPermission permission, StateMachineTransitionOperationRequest permissionRequest) {
            return permissionRequest.Modifier == permission.Modifier &&
                   permissionRequest.StateCaption == permission.StateCaption &&
                   permissionRequest.StateMachineName == permission.StateMachineName;
        }
    }
}