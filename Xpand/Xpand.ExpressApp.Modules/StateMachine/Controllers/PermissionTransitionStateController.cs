using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine.Xpo;
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
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            _changeStateActionController.RequestActiveState-=RequestActiveState;
        }

        bool IsActive(XpoTransition xpoTransition) {
            var permission = new StateMachineTransitionPermission {
                Modifier = StateMachineTransitionModifier.Allow,
                StateCaption = xpoTransition.TargetState.Caption,
                StateMachineName = xpoTransition.SourceState.StateMachine.Name,
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