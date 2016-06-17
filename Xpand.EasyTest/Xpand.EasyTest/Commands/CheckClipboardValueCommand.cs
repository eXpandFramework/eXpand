using System.Windows.Forms;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands {
    public class CheckClipboardValueCommand :Command{
        public const string Name = "CheckClipboardValue";
        protected override void InternalExecute(ICommandAdapter adapter){
            var text = Clipboard.GetText();
            var parameterValue = this.ParameterValue<string>("Value");
            if (text!=parameterValue){
                throw new TestException("Clipboard value is '"+text+"' which is not equal to '"+parameterValue+"'");
            }
        }
    }
}
