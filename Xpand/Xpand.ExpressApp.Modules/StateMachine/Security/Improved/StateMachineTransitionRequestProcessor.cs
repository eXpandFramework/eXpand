using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    public class StateMachineTransitionRequestProcessor : PermissionRequestProcessorBase<StateMachineTransitionOperationRequest> {
        readonly IPermissionDictionary _permissions;

        public StateMachineTransitionRequestProcessor(IPermissionDictionary permissions) {
            _permissions = permissions;
        }


        public override bool IsGranted(StateMachineTransitionOperationRequest permissionRequest) {
            var permission = _permissions.FindFirst<StateMachineTransitionPermission>();
            return permission == null || permissionRequest.Modifier == permission.Modifier &&
                   permissionRequest.StateCaption == permission.StateCaption &&
                   permissionRequest.StateMachineName == permission.StateMachineName;
        }
    }
}