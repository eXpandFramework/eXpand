using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Commands;
using Xpand.EasyTest;

namespace Xpand.ExpressApp.EasyTest.WebAdapter.Commands {
    public class XpandCheckValidationResultCommand: Xpand.EasyTest.Commands.XpandCheckValidationResultCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            var webCheckValidationResultCommand = new WebCheckValidationResultCommand();
            webCheckValidationResultCommand.SynchWith(this);
            webCheckValidationResultCommand.Execute(adapter);
        }
    }
}
