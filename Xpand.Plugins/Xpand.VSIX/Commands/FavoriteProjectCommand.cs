using System.ComponentModel.Design;
using System.Linq;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.ToolWindow.FavoriteProject;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands {
    public class FavoriteProjectCommand : VSCommand {
        private FavoriteProjectCommand() : base((sender, args) => VSPackage.VSPackage.Instance.ShowToolWindow<FavoriteProjectToolWindow>(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidFavoriteProject)){
            // this.EnableForDXSolution();
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            new FavoriteProjectCommand();

        }
    }
}