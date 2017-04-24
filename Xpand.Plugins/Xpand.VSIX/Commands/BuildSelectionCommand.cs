using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Commands{
    public class BuildSelectionCommand : IDTE2Provider{
        public static void Build(object sender, EventArgs e){
            new BuildSelectionCommand().Build();
        }

        private void Build(){
            var dte2 = this.DTE2();
            if (dte2.ActiveDocument != null){
                dte2.Windows.Item(Constants.vsext_wk_SProjectWindow).Activate();
                var uihSolutionExplorer = dte2.Windows.Item(Constants.vsext_wk_SProjectWindow).Object as UIHierarchy;
                if (uihSolutionExplorer != null && uihSolutionExplorer.UIHierarchyItems.Cast<UIHierarchyItem>().Any()){
                    
                }
                try{
                    var track =
                        dte2.Properties["Environment", "ProjectsAndSolution"].Item("TrackFileSelectionInExplorer");
                    if (track.Value is bool && !((bool) track.Value)){
                        dte2.WriteToOutput("aa");
                        track.Value = true;
                        track.Value = false;
                    }

                    // Find the Solution Explorer object
                    uihSolutionExplorer.Parent.Activate();
                    
                }
                catch (Exception ex){
                    if (ErrorHandler.IsCriticalException(ex))
                        throw;
                }

            }
        }
    }
}