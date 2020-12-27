using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class BuildSelectionCommand : VSCommand{
        private BuildSelectionCommand() : base((sender, args) =>Build() , new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidBuildSelection)){
            this.EnableForSolution();
            var dteCommand = Options.OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            var unused = new BuildSelectionCommand();
        }

        private static void Build(){
            var dte2 = DteExtensions.DTE;
            if (!FindInSolutionCommand.Find())
                dte2.Windows.Item(Constants.vsWindowKindSolutionExplorer).Activate();
            if (dte2.Windows.Item(Constants.vsext_wk_SProjectWindow).Object is UIHierarchy uihSolutionExplorer){
                var selectedHierarchyItems = ((UIHierarchyItem[]) uihSolutionExplorer.SelectedItems).ToArray();
                var solutionName = Path.GetFileNameWithoutExtension(dte2.Solution.FileName);
                if (selectedHierarchyItems.Length == 1 && selectedHierarchyItems.First().Name == Path.GetFileNameWithoutExtension(solutionName))
                    dte2.ExecuteCommand("Build.BuildSolution");
                else
                    dte2.ExecuteCommand("Build.BuildSelection");
            }
        }
    }
}