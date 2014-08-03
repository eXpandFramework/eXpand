using DevExpress.EasyTest.Framework;
using Xpand.Utils.Automation;

namespace Xpand.EasyTest.Commands{
    public class KillFocusCommand:Command{
        public const string Name = "KillFocus";
        protected override void InternalExecute(ICommandAdapter adapter){
            WindowAutomation.KillFocus();
        }
    }
}