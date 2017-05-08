using System.ComponentModel.Design;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class BuildSelectionCommand : VSCommand{
        private BuildSelectionCommand() : base((sender, args) =>Build() , new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidBuildSelection)){
            this.EnableForSolution();
            BindCommand("Global::Ctrl+Alt+Enter");
        }

        public static void Init(){
            new BuildSelectionCommand();
        }

        private static void Build(){
            if (FindInSolutionCommand.Find())
                DteExtensions.DTE.ExecuteCommand("Build.BuildSelection");
        }
    }
}