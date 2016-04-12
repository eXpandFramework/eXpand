namespace Xpand.Utils.Automation.InputSimulator{
    public interface IInputSimulator{
        IKeyboardSimulator Keyboard { get; }
        IMouseSimulator Mouse { get; }
        IInputDeviceStateAdaptor InputDeviceState { get; }
    }
}