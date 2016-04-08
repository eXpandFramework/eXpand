using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class FocusWindowCommand:Command{
        public const string Name = "FocusWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var activeWindowControl = adapter.CreateTestControl(TestControlType.Dialog, null);
            var activeWindowHandle = activeWindowControl.GetInterface<ITestWindow>().GetActiveWindowHandle();
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter("500");
            sleepCommand.Execute(adapter);
            if (!WindowAutomation.MakeTopMostWindow(activeWindowHandle))
                throw new AdapterOperationException("Cannot focus window " + activeWindowHandle.GetRootWindow().WindowText());
        }
    }
}