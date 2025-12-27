using System;
using DevExpress.ExpressApp.Security;
using Xpand.Persistent.Base.StateMachine;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    [Serializable]
    public class StateMachineTransitionOperationRequest : OperationPermissionRequestBase, IStateMachineTransitionPermission {
        public StateMachineTransitionOperationRequest(IStateMachineTransitionPermission permission)
            // : base(StateMachineTransitionPermission.OperationName) {
            : base("25.2 BC") {
            StateCaption = permission.StateCaption;
            StateMachineName = permission.StateMachineName;
            Hide = permission.Hide;
        }

        public string StateMachineName { get; set; }
        public string StateCaption { get; set; }
        public override object GetHashObject() {
            return "StateMachineTransitionOperationRequest";
        }

        public bool Hide { get; set; }
    }
}
