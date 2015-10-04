using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.StateMachine.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;
using StateMachineTransitionPermission = Xpand.ExpressApp.StateMachine.Security.Improved.StateMachineTransitionPermission;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class PermissionTransitionStateController:ViewController<ObjectView> {
        ChangeStateActionController _changeStateActionController;

        protected override void OnActivated() {
            base.OnActivated();
            _changeStateActionController = Frame.GetController<ChangeStateActionController>();
            _changeStateActionController.RequestActiveState+=RequestActiveState;
            var stateMachineController = Frame.GetController<StateMachineController>();
            stateMachineController.TransitionExecuting += OnTransitionExecuting;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _changeStateActionController.RequestActiveState-=RequestActiveState;
            var stateMachineController = Frame.GetController<StateMachineController>();
            stateMachineController.TransitionExecuting -= OnTransitionExecuting;
        }

        void OnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var transition = executeTransitionEventArgs.Transition;
            if (!executeTransitionEventArgs.Cancel && IsNotGranted(transition, false))
                throw new UserFriendlyException("Permissions are not granted for transitioning to the " + transition.Caption);
        }

        protected virtual bool IsNotGranted(ITransition iTransition, bool hide) {
            if (!SecuritySystem.IsGranted(new IsAdministratorPermissionRequest())){
                var permission = new StateMachineTransitionPermission {
                    StateCaption = iTransition.TargetState.Caption,
                    StateMachineName = iTransition.TargetState.StateMachine.Name,
                    Hide = hide,
                    Modifier = StateMachineTransitionModifier.Deny
                };
                return SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(permission));
            }
            return false;
        }

        void RequestActiveState(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof (PermissionTransitionStateController).Name;
            choiceActionItemArgs.Active[key] =!IsNotGranted(choiceActionItemArgs.Transition,true);
        }
    }
}