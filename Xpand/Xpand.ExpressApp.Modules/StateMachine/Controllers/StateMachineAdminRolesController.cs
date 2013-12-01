using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using Xpand.Persistent.Base.General;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine.Controllers {
    public class StateMachineAdminRolesController : StateMachineController {
        StateMachineAdminRolesController _stateMachineController;

        protected override void OnActivated() {
            base.OnActivated();
            if (Application.CanBuildSecurityObjects()) {
                _stateMachineController = Frame.GetController<StateMachineAdminRolesController>();
                _stateMachineController.TransitionExecuting+=StateMachineControllerOnTransitionExecuting;
            }
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (Application.CanBuildSecurityObjects())
                _stateMachineController.TransitionExecuting -= StateMachineControllerOnTransitionExecuting;
        }

        void StateMachineControllerOnTransitionExecuting(object sender, ExecuteTransitionEventArgs executeTransitionEventArgs) {
            var xpoStateMachine = (XpoStateMachine) executeTransitionEventArgs.Transition.TargetState.StateMachine;
            executeTransitionEventArgs.Cancel = !xpoStateMachine.CanExecuteTransition();
            if (executeTransitionEventArgs.Cancel)
                new StateMachineLogic(ObjectSpace).CallMethod("ProcessTransition", View.CurrentObject,
                    executeTransitionEventArgs.Transition.TargetState.StateMachine.StatePropertyName,
                    executeTransitionEventArgs.Transition.TargetState);
            
        }


    }
}