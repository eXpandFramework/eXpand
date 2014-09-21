using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;

namespace Xpand.EasyTest.Commands {
    public class SaveAndCloseCommand : Command {
        public const string Name = "SaveAndClose";
        protected override void InternalExecute(ICommandAdapter adapter) {
            if (adapter.IsWinAdapter()) {
                SaveAndCloseAction(adapter);
            }
            else {
                if (SaveAndCloseActionAvailable(adapter)) {
                    SaveAndCloseAction(adapter);
                }
                else {
                    var actionCommand = new ActionCommand();
                    actionCommand.Parameters.MainParameter = new MainParameter("OK");
                    actionCommand.Parameters.ExtraParameter = new MainParameter();
                    actionCommand.Execute(adapter);
                }
            }
        }

        private void SaveAndCloseAction(ICommandAdapter adapter) {
            var actionCommand = new ActionCommand();
            actionCommand.Parameters.MainParameter = new MainParameter("Save and Close");
            actionCommand.Parameters.ExtraParameter = new MainParameter();
            actionCommand.Execute(adapter);
        }

        private bool SaveAndCloseActionAvailable(ICommandAdapter adapter) {
            try {
                var actionAvailableCommand = new ActionAvailableCommand();
                actionAvailableCommand.Parameters.MainParameter = new MainParameter("Save and Close");
                actionAvailableCommand.Parameters.ExtraParameter = new MainParameter();
                actionAvailableCommand.Execute(adapter);
                return true;
            }
            catch (AdapterOperationException) {
                return false;
            }
        }
    }
}