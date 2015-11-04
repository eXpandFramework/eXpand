using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class DeleteCommand : ExecuteTableActionCommand {
        public const string Name = "Delete";
        protected override void InternalExecute(ICommandAdapter adapter) {
            base.InternalExecute(adapter);
            var sleepCommand = new SleepCommand();
            sleepCommand.Parameters.MainParameter = new MainParameter("1000");
            sleepCommand.Execute(adapter);

            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Delete");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);

            var handleDialogCommand = new XpandHandleDialogCommand();
            handleDialogCommand.Parameters.Add(new Parameter("Respond", "Yes", true, EndPosition));
            handleDialogCommand.Execute(adapter);
        }
    }
}