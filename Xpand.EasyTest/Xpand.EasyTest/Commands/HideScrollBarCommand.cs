using DevExpress.EasyTest.Framework;
using Xpand.Utils.Win32;

namespace Xpand.EasyTest.Commands{
    public abstract class HideScrollBarCommand:Command{
        public const string Name = "HideScrollBar";
        protected override void InternalExecute(ICommandAdapter adapter){
            var intPtr = Win32Declares.WindowFocus.GetForegroundWindow();
            Win32Declares.Window.ShowScrollBar(intPtr, Win32Declares.Window.ShowScrollBarEnum.SB_BOTH, false);
        }
    }
}