using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation.InputSimulator{
    internal interface IInputMessageDispatcher{
        void DispatchInput(Win32Types.INPUT[] inputs);
    }
}