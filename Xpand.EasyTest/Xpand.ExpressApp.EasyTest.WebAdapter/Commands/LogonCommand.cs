using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class LogonCommand:Xpand.EasyTest.Commands.LogonCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            var webCommandAdapter = ((WebCommandAdapter) adapter);
            var callbackTime = webCommandAdapter.WaitCallbackTime;
            var waitTimeoutTime = webCommandAdapter.WaitTimeoutTime;
            webCommandAdapter.WaitCallbackTime = 60000;
            webCommandAdapter.WaitTimeoutTime = 60000;
            base.InternalExecute(adapter);
            webCommandAdapter.WaitCallbackTime= callbackTime;
            webCommandAdapter.WaitTimeoutTime=waitTimeoutTime;
        }
    }
}
