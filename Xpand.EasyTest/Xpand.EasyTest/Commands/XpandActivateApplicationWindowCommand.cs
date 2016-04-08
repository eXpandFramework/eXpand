using System.Threading;
using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class XpandActivateApplicationWindowCommand : Command{
        public const string Name = "XpandActivateApplicationWindowCommand";
        protected override void InternalExecute(ICommandAdapter adapter) {
            var intPtr = adapter.GetApplicationWindowHandle();
            WindowAutomation.ForceWindowToForeground(intPtr);
            Thread.Sleep(3000);
        }

    }
}