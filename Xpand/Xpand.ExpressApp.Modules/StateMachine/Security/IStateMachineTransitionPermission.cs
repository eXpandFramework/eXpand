using System.Collections.Generic;

namespace Xpand.ExpressApp.StateMachine.Security {
    public interface IStateMachineTransitionPermission {
        StateMachineTransitionModifier Modifier { get; set; }
        string StateMachineName { get; set; }
        string StateCaption { get; set; }
        void SyncStateCaptions(IList<string> stateCaptions, string machineName);
        bool Hide { get; set; }
    }
}