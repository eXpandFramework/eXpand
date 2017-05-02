using System;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Commands{
    public class BuildSelectionCommand : IDTE2Provider{
        public static void Build(object sender, EventArgs e){
            new BuildSelectionCommand().Build();
        }

        private void Build(){
            var dte2 = this.DTE2();
            if (FindInSolutionCommand.Find())
                dte2.ExecuteCommand("Build.BuildSelection");
        }
    }
}