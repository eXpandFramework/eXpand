using System.ComponentModel;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;

namespace Xpand.ExpressApp.StateMachine.Security {
    public enum StateMachineTransitionModifier {
        Allow,
        Deny
    }

    [NonPersistent]
    public class StateMachineTransitionPermission : PermissionBase, INotifyPropertyChanged, IStateMachineTransitionPermission {
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(PropertyChangedEventArgs e) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        public override IPermission Copy() {
            return new StateMachineTransitionPermission(StateCaption, StateMachineName){Modifier = Modifier};
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

        public StateMachineTransitionModifier Modifier { get; set; }
        string _stateMachineName;

        [ImmediatePostData]
        public string StateMachineName {
            get { return _stateMachineName; }
            set {
                _stateMachineName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StateMachineName"));
            }
        }

        string _stateCaption;

        public string StateCaption {
            get { return _stateCaption; }
            set {
                _stateCaption = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StateCaption"));
            }
        }


        bool IStateMachineTransitionPermission.Hide { get; set; }
    }
}