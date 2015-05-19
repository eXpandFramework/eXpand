using System;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation.InputSimulator{
    public class WindowsInputDeviceStateAdaptor : IInputDeviceStateAdaptor {
        public bool IsKeyDown(Win32Constants.VirtualKeys keyCode){
            Int16 result = Win32Declares.KeyBoard.GetKeyState((UInt16) keyCode);
            return (result < 0);
        }

        public bool IsKeyUp(Win32Constants.VirtualKeys keyCode){
            return !IsKeyDown(keyCode);
        }

        public bool IsHardwareKeyDown(Win32Constants.VirtualKeys keyCode){
            int result = Win32Declares.KeyBoard.GetAsyncKeyState((UInt16) keyCode);
            return (result < 0);
        }

        public bool IsHardwareKeyUp(Win32Constants.VirtualKeys keyCode){
            return !IsHardwareKeyDown(keyCode);
        }

        public bool IsTogglingKeyInEffect(Win32Constants.VirtualKeys keyCode){
            Int16 result = Win32Declares.KeyBoard.GetKeyState((UInt16) keyCode);
            return (result & 0x01) == 0x01;
        }
    }
}