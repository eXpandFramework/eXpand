using System.Windows.Forms;
using DevExpress.EasyTest.Framework;

namespace Xpand.EasyTest.Commands{
    public class SendKeysCommand : Command{
        public const string Name = "SendKeys";
        protected override void InternalExecute(ICommandAdapter adapter){
            SendKeys.SendWait(Parameters.MainParameter.Value);
        }
    }
}