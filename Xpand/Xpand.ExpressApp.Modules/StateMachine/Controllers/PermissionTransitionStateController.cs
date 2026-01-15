using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.ExpressApp.StateMachine.Security.Improved;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Security;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class PermissionTransitionStateController:ViewController<ObjectView> {

        protected override void OnActivated() {
            base.OnActivated();
            // if (Application.Security.IsRemoteClient())
                // return ;
            if (!SecuritySystem.IsGranted(new IsAdministratorPermissionRequest())){
                Frame.GetController<StateMachineController>(controller => controller.TransitionExecuting += OnTransitionExecuting);
                Frame.GetController<ChangeStateActionController>(controller => controller.RequestActiveState += RequestActiveState);
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<StateMachineController>(controller => controller.TransitionExecuting -= OnTransitionExecuting);
            Frame.GetController<ChangeStateActionController>(controller => controller.RequestActiveState -= RequestActiveState);
        }

        void OnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs){
            var transition = executeTransitionEventArgs.Transition;
            if (!executeTransitionEventArgs.Cancel && IsGranted(transition, false))
                throw new UserFriendlyException("Permissions are not granted for transitioning to the " + transition.Caption);
        }

        protected virtual bool IsGranted(ITransition iTransition, bool hide) {
            // throw new NotImplementedException("25.2 BC");
            // var permission = new StateMachineTransitionPermission {
            //     StateCaption = iTransition.TargetState.Caption,
            //     StateMachineName = iTransition.TargetState.StateMachine.Name,
            //     Hide = hide,
            // };
            // return SecuritySystem.IsGranted(new NoCacheablePermissionRequest(new StateMachineTransitionOperationRequest(permission)));
        }

        void RequestActiveState(object sender, ChoiceActionItemArgs choiceActionItemArgs) {
            var key = typeof (PermissionTransitionStateController).Name;
            if (IsGranted(choiceActionItemArgs.Transition, true))
                choiceActionItemArgs.Active[key] =false;
        }
    }
}