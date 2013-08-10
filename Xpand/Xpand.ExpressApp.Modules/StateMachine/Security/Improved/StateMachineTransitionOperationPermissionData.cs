using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Security.Permissions;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;

namespace Xpand.ExpressApp.StateMachine.Security.Improved {
    [System.ComponentModel.DisplayName("StateMachineTransition")]
    public class StateMachineTransitionOperationPermissionData : XpandPermissionData, IStateMachineTransitionPermission {
        bool _hide;
        StateMachineTransitionModifier _modifier;

        string _stateCaption;

        IList<string> _stateCaptions = new List<string>();
        string _stateMachineName;


        public StateMachineTransitionOperationPermissionData(Session session)
            : base(session) {
        }

        [Browsable(false)]
        public IList<string> StateCaptions {
            get { return _stateCaptions; }
        }

        public StateMachineTransitionModifier Modifier {
            get { return _modifier; }
            set { SetPropertyValue("Modifier", ref _modifier, value); }
        }

        [ImmediatePostData]
        public string StateMachineName {
            get { return _stateMachineName; }
            set { SetPropertyValue("StateMachineName", ref _stateMachineName, value); }
        }

        [PropertyEditor(typeof (IStringLookupPropertyEditor))]
        [DataSourceProperty("StateCaptions")]
        public string StateCaption {
            get { return _stateCaption; }
            set { SetPropertyValue("StateCaption", ref _stateCaption, value); }
        }

        public void SyncStateCaptions(IList<string> stateCaptions, string machineName) {
            _stateCaptions = stateCaptions;
        }

        public bool Hide {
            get { return _hide; }
            set { SetPropertyValue("Hide", ref _hide, value); }
        }

        public override IList<IOperationPermission> GetPermissions() {
            return new IOperationPermission[]{new StateMachineTransitionPermission(this)};
        }
    }
}