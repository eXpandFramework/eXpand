using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.Xpo;
using System.Linq;
using Xpand.Persistent.Base.General;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine.Security {
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
            executeTransitionEventArgs.Cancel = !CanExecuteTransition((XpoStateMachine) executeTransitionEventArgs.Transition.TargetState.StateMachine);
            if (executeTransitionEventArgs.Cancel)
                new StateMachineLogic(ObjectSpace).CallMethod("ProcessTransition", View.CurrentObject,
                    executeTransitionEventArgs.Transition.TargetState.StateMachine.StatePropertyName,
                    executeTransitionEventArgs.Transition.TargetState);
            
        }

        public static bool CanExecuteTransition(XpoStateMachine stateMachine) {
            var collection = (XPBaseCollection) stateMachine.GetMemberValue(XpandStateMachineModule.AdminRoles);
            return collection.OfType<ISecurityRole>().Any(IsInRole);
        }

        static bool IsInRole(ISecurityRole securityRole) {
            return ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).Roles.Select(role => role.Name).Contains(securityRole.Name);
        }
    }
}