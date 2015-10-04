using DevExpress.ExpressApp.Security;
using System.Linq;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    public class StateMachineTransitionRequestProcessor : PermissionRequestProcessorBase<StateMachineTransitionOperationRequest> {
        readonly IPermissionDictionary _permissions;

        public StateMachineTransitionRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }

        public override bool IsGranted(StateMachineTransitionOperationRequest permissionRequest){
            var permissions = _permissions.GetPermissions<StateMachineTransitionPermission>().ToArray();
            return (!permissions.Any() || permissions.Any(permission => TransitionMatch(permission, permissionRequest)));
        }

        bool TransitionMatch(StateMachineTransitionPermission permission, StateMachineTransitionOperationRequest permissionRequest) {
            return permissionRequest.Modifier==permission.Modifier&&permissionRequest.Hide==permission.Hide&& permissionRequest.StateCaption == permission.StateCaption &&
                   permissionRequest.StateMachineName == permission.StateMachineName;
        }
    }
}