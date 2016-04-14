using System.Threading;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class XpandActivateApplicationWindowCommand : Command{
        public const string Name = "XpandActivateApplicationWindowCommand";
        protected override void InternalExecute(ICommandAdapter adapter) {
            var mainWindowHandle = adapter.GetMainWindowHandle();
            mainWindowHandle.ForceWindowToForeground();
            Thread.Sleep(3000);
        }

    }
}