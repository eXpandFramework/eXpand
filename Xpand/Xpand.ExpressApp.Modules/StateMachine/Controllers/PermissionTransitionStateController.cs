using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.StateMachine.Security.Improved;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class PermissionTransitionStateController:ViewController<ObjectView> {
        ChangeStateActionController _changeStateActionController;

        protected override void OnActivated() {
            base.OnActivated();
            if (!SecuritySystem.IsGranted(new IsAdministratorPermissionRequest())){
                var stateMachineController = Frame.GetController<StateMachineController>();
                stateMachineController.TransitionExecuting += OnTransitionExecuting;
                _changeStateActionController = Frame.GetController<ChangeStateActionController>();
                _changeStateActionController.RequestActiveState+=RequestActiveState;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (_changeStateActionController != null){
                _changeStateActionController.RequestActiveState-=RequestActiveState;
                var stateMachineController = Frame.GetController<StateMachineController>();
                stateMachineController.TransitionExecuting -= OnTransitionExecuting;
            }
        }

        void OnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs){
            var transition = executeTransitionEventArgs.Transition;
            if (!executeTransitionEventArgs.Cancel && IsGranted(transition, false))
                throw new UserFriendlyException("Permissions are not granted for transitioning to the " + transition.Caption);
        }

        protected virtual bool IsGranted(ITransition iTransition, bool hide) {
            var permission = new StateMachineTransitionPermission {
                StateCaption = iTransition.TargetState.Caption,
                StateMachineName = iTransition.TargetState.StateMachine.Name,
                Hide = hide,
            };
            return SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(permission));
        }

        void RequestActiveState(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof (PermissionTransitionStateController).Name;
            if (IsGranted(choiceActionItemArgs.Transition, true))
                choiceActionItemArgs.Active[key] =false;
        }
    }
}