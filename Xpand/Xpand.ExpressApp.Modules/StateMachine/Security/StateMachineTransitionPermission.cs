using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.StateMachine;

namespace Xpand.ExpressApp.StateMachine.Security {

    [NonPersistent]
    public class StateMachineTransitionPermission : PermissionBase, IStateMachineTransitionPermission {
        
        
        public override IPermission Copy() {
            return new StateMachineTransitionPermission(StateCaption, StateMachineName);
        }

        public StateMachineTransitionPermission() {
        }
        public StateMachineTransitionPermission(string stateCaption, string stateMachineName) {
            StateCaption = stateCaption;
            StateMachineName = stateMachineName;
        }
        public override bool IsSubsetOf(IPermission target) {
            var isSubsetOf = base.IsSubsetOf(target);
            if (isSubsetOf) {
                var stateMachineTransitionPermission = ((StateMachineTransitionPermission)target);
                return stateMachineTransitionPermission.StateCaption == StateCaption &&
                       stateMachineTransitionPermission.StateMachineName == StateMachineName;
            }
            return false;
        }

        string _stateMachineName;

        [ImmediatePostData]
        public string StateMachineName {
            get => _stateMachineName;
            set {
                _stateMachineName = value;
                OnPropertyChanged(nameof(StateMachineName));
            }
        }

        string _stateCaption;

        public string StateCaption {
            get => _stateCaption;
            set {
                _stateCaption = value;
                OnPropertyChanged(nameof(StateCaption));
            }
        }


        bool IStateMachineTransitionPermission.Hide { get; set; }
    }
}