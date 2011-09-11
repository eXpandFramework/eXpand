using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.StateMachine.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General.CustomAttributes;
using PermissionBase = Xpand.ExpressApp.Security.Permissions.PermissionBase;
using Xpand.Persistent.Base.General;

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

        IObjectSpace _objectSpace;
        IList<string> _stateCaptions;

        [Browsable(false)]
        public IList<string> StateCaptions {
            get {
                return _objectSpace != null ? _stateCaptions : new List<string>();
            }
        }

        public void SyncInfo(IObjectSpace objectSpace, string machineName) {
            _objectSpace = objectSpace;
            StateMachineName = machineName;
            _stateCaptions = _objectSpace.GetObjects<XpoState>(state => state.StateMachine.Name == StateMachineName).Select(
                    state => state.Caption).ToList().AsReadOnly();
        }
    }
}