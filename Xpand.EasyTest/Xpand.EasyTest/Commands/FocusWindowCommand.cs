using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class FocusWindowCommand:Command{
        public const string Name = "FocusWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            var activeWindowHandle = activeWindowControl.GetInterface<ITestWindow>().GetActiveWindowHandle();
            WindowAutomation.FocusWindow(activeWindowHandle);
        }
    }
}