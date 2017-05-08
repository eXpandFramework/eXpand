using System.ComponentModel.Design;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;
using Xpand.VSIX.Wizard;

namespace Xpand.VSIX.Commands{
    public class AddXpandReferencesCommand:VSCommand{
        private AddXpandReferencesCommand() : base((sender, args) => SolutionWizard.Show(),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidAddXpandReference)){
            this.ActiveForDXSolution();
        }

        public static void Init(){
            new AddXpandReferencesCommand();
        }
    }
}