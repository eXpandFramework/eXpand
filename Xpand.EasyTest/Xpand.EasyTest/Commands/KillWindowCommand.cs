using System;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class KillWindowCommand:Command{
        public const string Name = "KillWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var mainParameter = Parameters.MainParameter;
            IntPtr windowHandle = Win32.GetForegroundWindow();
            if (!string.IsNullOrEmpty(mainParameter.Value)){
                windowHandle = Win32.FindWindowByCaption(IntPtr.Zero, mainParameter.Value);
                if (windowHandle == IntPtr.Zero)
                    throw new CommandException(String.Format("Cannot find window {0}", mainParameter.Value),StartPosition);
            }
            Win32.DestroyWindow(windowHandle);
        }
    }
}