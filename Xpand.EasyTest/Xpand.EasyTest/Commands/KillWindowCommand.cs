using System;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public class KillWindowCommand:Command{
        public const string Name = "KillWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var mainParameter = Parameters.MainParameter;
            IntPtr windowHandle = Win32Declares.WindowFocus.GetForegroundWindow();
            if (!string.IsNullOrEmpty(mainParameter.Value)){
                windowHandle = Win32Declares.WindowHandles.FindWindowByCaption(IntPtr.Zero, mainParameter.Value);
                if (windowHandle == IntPtr.Zero)
                    throw new CommandException(String.Format("Cannot find window {0}", mainParameter.Value),StartPosition);
            }
            WindowAutomation.CloseWindow(windowHandle);
        }
    }
}