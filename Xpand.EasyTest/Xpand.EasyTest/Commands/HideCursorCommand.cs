using DevExpress.EasyTest.Framework;
using Command = DevExpress.EasyTest.Framework.Command;

namespace Xpand.EasyTest.Commands{
    public class HideCursorCommand:Command{
        public const string Name = "HideCursor";

        protected override void InternalExecute(ICommandAdapter adapter){
            System.Windows.Forms.Cursor.Hide();
        }
    }
}