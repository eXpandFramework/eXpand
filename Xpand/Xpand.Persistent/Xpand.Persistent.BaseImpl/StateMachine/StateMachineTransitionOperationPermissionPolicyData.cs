using System.Collections.Generic;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.Base.StateMachine;
using Xpand.Persistent.BaseImpl.Security.PermissionPolicyData;

namespace Xpand.Persistent.BaseImpl.StateMachine {
    [System.ComponentModel.DisplayName("StateMachineTransition")]
    public class StateMachineTransitionOperationPermissionPolicyData : PermissionPolicyData, IStateMachineTransitionPermission {
        bool _hide;

        string _stateCaption;

        string _stateMachineName;


        public StateMachineTransitionOperationPermissionPolicyData(Session session)
            : base(session) {
        }

        public StateMachineTransitionModifier Modifier => StateMachineTransitionModifier.Deny;

        [ImmediatePostData]
        public string StateMachineName {
            get { return _stateMachineName; }
            set { SetPropertyValue("StateMachineName", ref _stateMachineName, value); }
        }

        [DataSourceProperty("StateCaptions")]
        public string StateCaption {
            get { return _stateCaption; }
            set { SetPropertyValue("StateCaption", ref _stateCaption, value); }
        }

        public bool Hide {
            get { return _hide; }
            set { SetPropertyValue("Hide", ref _hide, value); }
        }

        public override IList<IOperationPermission> GetPermissions(){
            return GetOperationPermissions();
        }

        private IList<IOperationPermission> GetOperationPermissions(){
            var operationPermissions = GetOperationPermissions<IStateMachineTransitionPermission>();
            return operationPermissions;
        }
    }
}