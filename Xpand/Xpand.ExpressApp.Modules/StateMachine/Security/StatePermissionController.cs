using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace Xpand.ExpressApp.StateMachine.Security {
    public interface IModelOptionsStateMachine {
        bool PermissionsForActionState { get; set; }
    }
    public class StatePermissionController : ViewController, IModelExtender {
        void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelOptions, IModelOptionsStateMachine>();
        }

        protected override void OnActivated() {
            base.OnActivated();
            var stateMachineController = Frame.GetController<StateMachineController>();
            stateMachineController.TransitionExecuting += OnTransitionExecuting;
        }

        void OnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var states = executeTransitionEventArgs.Transition.TargetState.StateMachine.States.OfType<XpoState>();
            foreach (var state in states) {
                if (IsNotGranted(state))
                    throw new UserFriendlyException("Permissions are not granted for transitioning to the " + state.Caption);
            }
        }

        bool IsNotGranted(XpoState state) {
            if (!((ISecurityComplex)SecuritySystem.Instance).IsNewSecuritySystem())
                return IsNotGranted(new StateMachineTransitionPermission(StateMachineTransitionModifier.Deny, state.Caption, state.StateMachine.Name));
            var stateMachineTransitionPermission = new Improved.StateMachineTransitionPermission {
                Modifier = StateMachineTransitionModifier.Deny,
                StateCaption = state.Caption,
                StateMachineName = state.StateMachine.Name
            };
            return !SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(stateMachineTransitionPermission));
        }

        static bool IsNotGranted(IPermission permission) {
            var securityComplex = ((SecurityBase)SecuritySystem.Instance);
            bool isGrantedForNonExistentPermission = securityComplex.IsGrantedForNonExistentPermission;
            securityComplex.IsGrantedForNonExistentPermission = true;
            bool granted = SecuritySystem.IsGranted(permission);
            securityComplex.IsGrantedForNonExistentPermission = isGrantedForNonExistentPermission;
            return granted;
        }
    }
}