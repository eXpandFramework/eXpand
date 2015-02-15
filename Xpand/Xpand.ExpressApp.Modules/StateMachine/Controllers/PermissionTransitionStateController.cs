using DevExpress.ExpressApp;
using Xpand.ExpressApp.StateMachine.Security;
using Xpand.ExpressApp.StateMachine.Security.Improved;
using StateMachineTransitionPermission = Xpand.ExpressApp.StateMachine.Security.Improved.StateMachineTransitionPermission;
using DevExpress.ExpressApp.StateMachine;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class PermissionTransitionStateController:ViewController<ObjectView> {
        ChangeStateActionController _changeStateActionController;

        protected override void OnActivated() {
            base.OnActivated();
            _changeStateActionController = Frame.GetController<ChangeStateActionController>();
            _changeStateActionController.RequestActiveState+=RequestActiveState;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _changeStateActionController.RequestActiveState-=RequestActiveState;
        }

        protected virtual bool IsActive(ITransition iTransition) {
            var permission = new StateMachineTransitionPermission {
                Modifier = StateMachineTransitionModifier.Allow,
                StateCaption = iTransition.TargetState.Caption,
                StateMachineName = iTransition.TargetState.StateMachine.Name,
                Hide = false
            };
            return SecuritySystem.IsGranted(new StateMachineTransitionOperationRequest(permission));
        }

        void RequestActiveState(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof (PermissionTransitionStateController).Name;
            choiceActionItemArgs.Active[key] =IsActive(choiceActionItemArgs.Transition);
        }
    }
}