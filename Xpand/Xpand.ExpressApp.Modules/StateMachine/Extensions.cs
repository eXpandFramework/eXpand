using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.Xpo;
using Fasterflect;

namespace Xpand.ExpressApp.StateMachine {
    public static class Extensions {
        public static bool CanExecuteAllTransitions(this IStateMachine stateMachine) {
            var collection = (XPBaseCollection)((XPBaseObject)stateMachine).GetMemberValue(XpandStateMachineModule.AdminRoles);
            return collection.OfType<ISecurityRole>().Any() && collection.OfType<ISecurityRole>().Any(IsInRole);
        }
        static bool IsInRole(ISecurityRole securityRole) {
            return ((ISecurityUserWithRoles)SecuritySystem.CurrentUser).Roles.Select(role => role.Name).Contains(securityRole.Name);
        }

        public static Dictionary<object, List<SimpleAction>> PanelActions(this StateMachineController stateMachineController) {
            return (Dictionary<object, List<SimpleAction>>) stateMachineController.GetFieldValue("panelActions");
        }

        public static void UpdateActionState(this StateMachineController stateMachineController){
            stateMachineController.CallMethod("UpdateActionState");
        }
    }
}
