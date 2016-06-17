using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using Xpand.EasyTest;

namespace Xpand.ExpressApp.EasyTest.WinAdapter.Commands {
    public class XpandCheckValidationResultCommand : Xpand.EasyTest.Commands.XpandCheckValidationResultCommand {
        protected override void InternalExecute(ICommandAdapter adapter){
            var winCheckValidationResultCommand = new WinCheckValidationResultCommand();
            winCheckValidationResultCommand.SynchWith(this);
            winCheckValidationResultCommand.Execute(adapter);
            var actionCommand = new ActionCommand();
            actionCommand.DoAction(adapter, "Close",null);
        }
    }
}
