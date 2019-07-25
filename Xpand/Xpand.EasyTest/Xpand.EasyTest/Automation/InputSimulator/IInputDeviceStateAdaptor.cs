using Xpand.EasyTest.Win32;

namespace Xpand.EasyTest.Automation.InputSimulator{
    public interface IInputDeviceStateAdaptor{
        bool IsKeyDown(Win32Constants.VirtualKeys keyCode);
        bool IsKeyUp(Win32Constants.VirtualKeys keyCode);
        bool IsHardwareKeyDown(Win32Constants.VirtualKeys keyCode);
        bool IsHardwareKeyUp(Win32Constants.VirtualKeys keyCode);
        bool IsTogglingKeyInEffect(Win32Constants.VirtualKeys keyCode);
    }
}