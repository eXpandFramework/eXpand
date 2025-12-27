// using DevExpress.ExpressApp.Security;
// using System.Linq;
// using Xpand.ExpressApp.Security.Permissions;
//
// namespace Xpand.ExpressApp.StateMachine.Security.Improved {
//     public class StateMachineTransitionRequestProcessor : PermissionRequestProcessorBase<StateMachineTransitionOperationRequest>,ICustomPermissionRequestProccesor {
//
//
//         public override bool IsGranted(StateMachineTransitionOperationRequest permissionRequest){
//             var permissions = Permissions.GetPermissions<StateMachineTransitionPermission>().ToArray();
//             return (permissions.Any(permission => TransitionMatch(permission, permissionRequest)));
//         }
//
//         bool TransitionMatch(StateMachineTransitionPermission permission, StateMachineTransitionOperationRequest permissionRequest) {
//             return permissionRequest.Hide==permission.Hide&& permissionRequest.StateCaption == permission.StateCaption &&
//                    permissionRequest.StateMachineName == permission.StateMachineName;
//         }
//
//         public IPermissionDictionary Permissions{ get; set; }
//     }
// }