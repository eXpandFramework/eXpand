using System;
using System.Runtime.InteropServices;
using Xpand.Utils.Win32;

namespace Xpand.Utils.Automation.InputSimulator{
    internal class WindowsInputMessageDispatcher : IInputMessageDispatcher{
        public void DispatchInput(Win32Types.INPUT[] inputs){
            if (inputs == null) throw new ArgumentNullException("inputs");
            if (inputs.Length == 0) throw new ArgumentException("The input array was empty", "inputs");
            uint successful = Win32Declares.KeyBoard.SendInput((UInt32) inputs.Length, inputs,
                Marshal.SizeOf(new Win32Types.INPUT()));
            if (successful != inputs.Length)
                throw new Exception(
                    "Some simulated input commands were not sent successfully. " +
                    "The most common reason for this happening are the security features of Windows " +
                    "including User Interface Privacy Isolation (UIPI). " +
                    "Your application can only send commands to applications of the same or lower elevation. " +
                    "Similarly certain commands are restricted to Accessibility/UIAutomation applications. " +
                    "Refer to the project home page and the code samples for more information.");
        }
    }
}