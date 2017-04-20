using System;
using System.Diagnostics;
using System.Linq;
using Xpand.VSIX.Extensions;

namespace Xpand.VSIX.Commands{
    public class IISExpress{
        public static void KillIISExpress(object sender, EventArgs eventArgs){
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