using System.Collections.Generic;
using DevExpress.ExpressApp.Security;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    public class StateMachineTransitionPermission : OperationPermissionBase, IStateMachineTransitionPermission {
        public const string OperationName = "StateMachineTransition";
        public StateMachineTransitionPermission(IStateMachineTransitionPermission permission)
            : base(OperationName) {
            StateCaption=permission.StateCaption;
            StateMachineName=permission.StateMachineName;
            Modifier=permission.Modifier;
        }

        public StateMachineTransitionPermission() : base(OperationName) {
        }

        public override IList<string> GetSupportedOperations() {
            return new[] { OperationName };
        }
        public StateMachineTransitionModifier Modifier { get; set; }
        public string StateMachineName { get; set; }
        public string StateCaption { get; set; }

        void IStateMachineTransitionPermission.SyncStateCaptions(IList<string> stateCaptions, string machineName) {
            
        }
    }
}
