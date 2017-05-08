using System.ComponentModel.Design;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.ModelEditor;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands {
    public class ShowModelsWindowCommand:VSCommand {
        private ShowModelsWindowCommand() : base((sender, args) => VSPackage.VSPackage.Instance.ShowToolWindow<ModelToolWindow>(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidShowMEToolbox)){
            this.EnableForDXSolution();
            BindCommand("Global::Ctrl+Alt+Shift+M");
        }

        public static void Init(){
            new ShowModelsWindowCommand();

        }
    }
}
