using System.ComponentModel.Design;
using System.Linq;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class ShowOptionsCommand:VSCommand{
        private ShowOptionsCommand() : base((sender, args) => VSPackage.VSPackage.Instance.ShowOptionPage(typeof(OptionsPage)),new CommandID(PackageGuids.guidVSXpandPackageCmdSet,PackageIds.cmdidOptions)){
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            new ShowOptionsCommand();
        }
    }
}