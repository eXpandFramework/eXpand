using Xpand.EasyTest.Win32;

namespace Xpand.EasyTest.Automation.InputSimulator{
    internal interface IInputMessageDispatcher{
        void DispatchInput(Win32Types.INPUT[] inputs);
    }
}