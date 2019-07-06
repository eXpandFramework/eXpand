using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.Automation;

namespace Xpand.EasyTest.Commands{
    public class MinimizeApplicationWindowCommand : Command{
        public const string Name = "MinimizeApplicationWindow";
        protected override void InternalExecute(ICommandAdapter adapter){
            var mainWindowHandle = adapter.GetMainWindowHandle();
            mainWindowHandle.MinimizeWindow();
        }
    }
}