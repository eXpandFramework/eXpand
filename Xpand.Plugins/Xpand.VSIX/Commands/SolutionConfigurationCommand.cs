using System.ComponentModel.Design;
using System.Linq;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.ToolWindow.SolutionConfiguration;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands {
    public class SolutionConfigurationCommand : VSCommand {
        private SolutionConfigurationCommand() : base(
            (sender, args) => VSPackage.VSPackage.Instance.ShowToolWindow<SolutionConfigurationToolWindow>(),
            new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidSolutionConfiguration)) {
            // this.EnableForDXSolution();
            var dteCommand =
                OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            new SolutionConfigurationCommand();

        }
    }

}