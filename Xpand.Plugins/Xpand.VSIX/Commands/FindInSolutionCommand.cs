using System;
using Microsoft.VisualStudio;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Commands{
    public class FindInSolutionCommand:IDTE2Provider{
        public static bool Find(){
            var findInSolutionCommand = new FindInSolutionCommand();
            return findInSolutionCommand.FindCore();
        }

        private bool FindCore(){
            var dte2 = this.DTE2();
            if (dte2.ActiveDocument != null) {
                try {
                    var track =
                        dte2.Properties["Environment", "ProjectsAndSolution"].Item("TrackFileSelectionInExplorer");
                    if (track.Value is bool && !((bool)track.Value)) {
                        track.Value = true;
                        track.Value = false;
                    }
                    return true;
                }
                catch (Exception ex) {
                    if (ErrorHandler.IsCriticalException(ex))
                        throw;
                }

            }

            return false;
        }
    }
}