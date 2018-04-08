using System.ComponentModel.Design;
using System.Linq;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands {
    public class ShowModelsWindowCommand:VSCommand {
        private ShowModelsWindowCommand() : base((sender, args) => VSPackage.VSPackage.Instance.ShowToolWindow<ModelToolWindow>(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidShowMEToolbox)){
            this.EnableForDXSolution();
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            if (!string.IsNullOrWhiteSpace(dteCommand?.Shortcut))
                BindCommand(dteCommand.Shortcut);
        }

        public static void Init(){
            new ShowModelsWindowCommand();

        }
    }
}
