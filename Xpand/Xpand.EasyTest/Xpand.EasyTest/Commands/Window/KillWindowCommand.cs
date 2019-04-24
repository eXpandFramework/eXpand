using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands.Window{
    public class KillWindowCommand:WindowCommand{
        public const string Name = "KillWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var windowHandle = GetWindowHandle(adapter);
            windowHandle.CloseWindow();
        }
    }
}