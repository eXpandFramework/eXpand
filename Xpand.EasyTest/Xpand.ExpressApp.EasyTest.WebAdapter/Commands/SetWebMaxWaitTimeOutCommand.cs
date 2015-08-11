using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Commands;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class SetWebMaxWaitTimeOutCommand : Xpand.EasyTest.Commands.SetWebMaxWaitTimeOutCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            var webMaxWaitTimeCommand = new SetMaxWaitTimeoutTimeCommand();
            webMaxWaitTimeCommand.Parameters.Add(new Parameter("Timeout",Parameters.MainParameter.Value,true,StartPosition));
            webMaxWaitTimeCommand.Execute(adapter);

            var setMaxWaitCallbackTimeCommand = new SetMaxWaitCallbackTimeCommand();
            setMaxWaitCallbackTimeCommand.Parameters.Add(new Parameter("Timeout", Parameters.MainParameter.Value, true, StartPosition));
            setMaxWaitCallbackTimeCommand.Execute(adapter);
        }
    }
}
