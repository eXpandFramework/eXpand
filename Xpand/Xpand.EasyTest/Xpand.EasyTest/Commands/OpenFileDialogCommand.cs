using DevExpress.EasyTest.Framework;
using Xpand.EasyTest.Commands.Window;

namespace Xpand.EasyTest.Commands{
    public class OpenFileDialogCommand:Command {
        public const string Name = "OpenFileDialog";
        protected override void InternalExecute(ICommandAdapter adapter){
            var xpandFillFormCommand = new XpandFillFormCommand();
            xpandFillFormCommand.Parameters.Add(new Parameter("File name:", Parameters.MainParameter.Value, true, EndPosition));
            xpandFillFormCommand.Execute(adapter);
            var handleDialogCommand = new XpandHandleDialogCommand();
            handleDialogCommand.Parameters.Add(new Parameter("Respond", ButtonLocalizations.GetLocalizedButtonCaption("Open"), true, EndPosition));
            handleDialogCommand.Execute(adapter);
        }
    }
}