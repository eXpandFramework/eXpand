using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.VSPackage;
using ConfigurationProperty = Xpand.VSIX.Extensions.ConfigurationProperty;

namespace Xpand.VSIX.Commands {
    public class XAFErrorExplorerCommand :VSCommand{
        private XAFErrorExplorerCommand():base((sender, args) => Explore(), new CommandID(PackageGuids.guidVSXpandPackageCmdSet, PackageIds.cmdidExploreXAFErrors)){
            this.EnableForDXSolution();
        }

        public static void Init(){
            new XAFErrorExplorerCommand();
        }

        public static void Explore(){
            Project startUpProject = DteExtensions.DTE.Solution.FindStartUpProject();
            Property outPut = startUpProject.ConfigurationManager.ActiveConfiguration.FindProperty(ConfigurationProperty.OutputPath);
            bool isWeb = startUpProject.IsWeb();
            string fullPath = startUpProject.FindProperty(ProjectProperty.FullPath).Value + "";
            string path = Path.Combine(fullPath, outPut.Value.ToString()) + "";
            if (isWeb)
                path = Path.GetDirectoryName(startUpProject.FullName);
            Func<Stream> streamSource = () => {
                var path1 = path + "";
                File.Copy(Path.Combine(path1, "expressAppFrameWork.log"), Path.Combine(path1, "expressAppFrameWork.locked"), true);
                return File.Open(Path.Combine(path1, "expressAppFrameWork.locked"), FileMode.Open, FileAccess.Read, FileShare.Read);
            };
            var reader = new ReverseLineReader(streamSource);
            var stackTrace = new List<string>();
            foreach (var readline in reader) {
                stackTrace.Add(readline);
                if (readline.Trim().StartsWith("The error occured:") || readline.Trim().StartsWith("The error occurred:")) {
                    stackTrace.Reverse();
                    string errorMessage = "";
                    foreach (string trace in stackTrace) {
                        errorMessage += trace + Environment.NewLine;
                        if (trace.Trim().StartsWith("----------------------------------------------------"))
                            break;
                    }
                    Clipboard.SetText(errorMessage);
                    break;
                }
            }

        }

    }
}
