using System.IO;
using EnvDTE;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Process = System.Diagnostics.Process;

namespace Xpand.VSIX.Commands {
    class ProjectConverter {
        private static readonly DTE _dte=DteExtensions.DTE;
        private static string GetProjectConverterPath() {
            if (string.IsNullOrWhiteSpace(OptionClass.Instance.ProjectConverterPath)) {
                var version = _dte.Solution.GetDXVersion();
                var dxRootDirectory = _dte.Solution.GetDXRootDirectory();
                return Path.Combine(dxRootDirectory + @"\Tools\Components", "TestExecutor." + version + ".exe");
            }
            return OptionClass.Instance.ProjectConverterPath;
        }
        public static void Convert(){
            _dte.InitOutputCalls("ConvertProject");
            string path = GetProjectConverterPath();
            string token = OptionClass.Instance.Token;
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(token)) {
                var directoryName = Path.GetDirectoryName(_dte.Solution.FileName);
                _dte.WriteToOutput("Project Converter Started !!!");
                var userName = $"/sc /k:{token} \"{directoryName}\"";
                Process.Start(path, userName);
            }
        }

    }
}
