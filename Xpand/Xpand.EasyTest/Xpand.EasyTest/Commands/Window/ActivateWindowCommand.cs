using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands.Window{
    public class ActivateWindowCommand : WindowCommand{
        public const string Name = "ActivateWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var windowHandle = GetWindowHandle(adapter);
            windowHandle.ForceWindowToForeground();
        }
    }
}