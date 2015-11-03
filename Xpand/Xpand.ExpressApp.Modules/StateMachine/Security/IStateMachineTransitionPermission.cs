namespace Xpand.ExpressApp.StateMachine.Security {
    public interface IStateMachineTransitionPermission {
        string StateMachineName { get; set; }
        string StateCaption { get; set; }
        bool Hide { get; set; }
    }
}