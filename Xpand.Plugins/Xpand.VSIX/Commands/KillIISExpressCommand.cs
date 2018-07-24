using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Xpand.VSIX.VSPackage;

namespace Xpand.VSIX.Commands{
    public class KillIISExpressCommand:VSCommand{
        private KillIISExpressCommand() : base((sender, args) => Kill(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidKillIISExpress)) {
            var dteCommand = OptionClass.Instance.DteCommands.FirstOrDefault(command => command.Command == GetType().Name);
            BindCommand(dteCommand);
        }

        public static void Init(){
            var unused = new KillIISExpressCommand();
        }

        public static void Kill(){
            var processes = Process.GetProcessesByName("IISExpress");
            if (!processes.Any())
                DteExtensions.DTE.WriteToOutput("IISEXpress is not running");
            else {
                foreach (var process in processes) {
                    var processId = process.Id;
                    DteExtensions.DTE.WriteToOutput($"Killing IISExpress({processId})");
                    process.Kill();
                    DteExtensions.DTE.WriteToOutput($"IISExpress({processId}) stopped");
                }
            }
        }
    }
}