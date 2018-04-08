using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class FindInSolutionCommand:VSCommand{
        private FindInSolutionCommand() : base((sender, args) =>Find(),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidFindInSolution) ){
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            if (!string.IsNullOrWhiteSpace(dteCommand?.Shortcut))
                BindCommand(dteCommand.Shortcut);
            this.EnableForSolution();
        }

        public static bool Find(){
            var dte2 = DteExtensions.DTE;
            if (dte2.ActiveDocument != null) {
                try {
                    var track =
                        dte2.Properties["Environment", "ProjectsAndSolution"].Item("TrackFileSelectionInExplorer");
                    if (track.Value is bool b && !b) {
                        track.Value = true;
                        track.Value = false;
                    }
                    dte2.Windows.Item(Constants.vsWindowKindSolutionExplorer).Activate();
                    return true;
                }
                catch (Exception ex) {
                    if (ErrorHandler.IsCriticalException(ex))
                        throw;
                }
            }
            return false;

        }

        public static void Init(){
            // ReSharper disable once ObjectCreationAsStatement
            new FindInSolutionCommand();
        }
    }
}