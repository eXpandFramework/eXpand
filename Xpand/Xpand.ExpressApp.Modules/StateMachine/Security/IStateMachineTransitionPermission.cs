namespace Xpand.ExpressApp.StateMachine.Security {
    public interface IStateMachineTransitionPermission {
        StateMachineTransitionModifier Modifier { get; set; }
        string StateMachineName { get; set; }
        string StateCaption { get; set; }
        bool Hide { get; set; }
    }
}