using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class LogonCommand:Xpand.EasyTest.Commands.LogonCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            var webCommandAdapter = ((WebCommandAdapter) adapter);
            var waitTimeoutTime = webCommandAdapter.WaitTimeoutTime;
            webCommandAdapter.WaitTimeoutTime = 120000;
            base.InternalExecute(adapter);
            webCommandAdapter.WaitTimeoutTime=waitTimeoutTime;
        }
    }
}
