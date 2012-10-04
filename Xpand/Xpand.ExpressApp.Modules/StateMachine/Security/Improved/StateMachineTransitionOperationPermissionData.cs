using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    [System.ComponentModel.DisplayName("StateMachineTransition")]
    public class StateMachineTransitionOperationPermissionData : XpandPermissionData, IStateMachineTransitionPermission {
        public StateMachineTransitionOperationPermissionData(Session session)
            : base(session) {
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[] { new StateMachineTransitionPermission(this) };
        }
        private StateMachineTransitionModifier _modifier;
        public StateMachineTransitionModifier Modifier {
            get {
                return _modifier;
            }
            set {
                SetPropertyValue("Modifier", ref _modifier, value);
            }
        }
        private string _stateMachineName;
        [ImmediatePostData]
        public string StateMachineName {
            get {
                return _stateMachineName;
            }
            set {
                SetPropertyValue("StateMachineName", ref _stateMachineName, value);
            }
        }
        private string _stateCaption;
        [PropertyEditor(typeof(IStringLookupPropertyEditor))]
        [DataSourceProperty("StateCaptions")]
        public string StateCaption {
            get {
                return _stateCaption;
            }
            set {
                SetPropertyValue("StateCaption", ref _stateCaption, value);
            }
        }
        IList<string> _stateCaptions = new List<string>();
        [Browsable(false)]
        public IList<string> StateCaptions { get { return _stateCaptions; } }

        public void SyncStateCaptions(IList<string> stateCaptions, string machineName) {
            _stateCaptions = stateCaptions;
        }
    }
}