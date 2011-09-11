using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General.CustomAttributes;
using PermissionBase = Xpand.ExpressApp.Security.Permissions.PermissionBase;

namespace Xpand.ExpressApp.StateMachine {
    public enum StateMachineTransitionModifier {
        Allow,
        Deny
    }

    [NonPersistent]
    public class StateMachineTransitionPermission : PermissionBase {
        public override IPermission Copy() {
            return new StateMachineTransitionPermission(Modifier, StateCaption, StateMachineName);
        }

        public StateMachineTransitionPermission() {
        }
        public StateMachineTransitionPermission(StateMachineTransitionModifier modifier, string stateCaption, string stateMachineName) {
            Modifier = modifier;
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
        [ImmediatePostData]
        public string StateMachineName { get; set; }

        [PropertyEditor(typeof(IStringLookupPropertyEditor))]
        [DataSourceProperty("StateCaptions")]
        public string StateCaption { get; set; }

        IList<string> _stateCaptions = new List<string>();
        [Browsable(false)]
        public IList<string> StateCaptions {get {return _stateCaptions;}}

        public void SyncStateCaptions(IList<string> stateCaptions, string machineName) {
            StateMachineName = machineName;
            _stateCaptions = stateCaptions;
        }
    }
}