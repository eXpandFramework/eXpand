using System;
using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.Win32;

namespace Xpand.EasyTest.Commands.Window{
    public abstract class WindowCommand:Command{
        protected IntPtr GetWindowHandle(ICommandAdapter adapter) {
            var caption = this.ParameterValue<string>("WindowCaption");
            return !string.IsNullOrEmpty(caption)
                ? Win32Declares.WindowHandles.FindWindowByCaption(IntPtr.Zero, caption)
                : adapter.GetMainWindowHandle();
        }
    }
}