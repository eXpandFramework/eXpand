using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using EnvDTE80;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Process = System.Diagnostics.Process;

namespace Xpand.VSIX.ToolWindow.ModelEditor {
    public class ModelEditorRunner {
        private readonly DTE2 _dte = DteExtensions.DTE;

        public void Start(ProjectItemWrapper projectItemWrapper) {
            string outputFileName = projectItemWrapper.OutputFileName;
            var fullPath = projectItemWrapper.FullPath;
            string assemblyPath = Path.Combine(fullPath, Path.Combine(projectItemWrapper.OutputPath, outputFileName));
            if (!File.Exists(assemblyPath)) {
                MessageBox.Show($@"Assembly {assemblyPath} not found", null, MessageBoxButtons.OK);
                return;
            }
            string mePath = null;
            if (projectItemWrapper.TargetFramework != null && (projectItemWrapper.TargetFramework.StartsWith("netcore") ||
                                                               projectItemWrapper.TargetFramework == "netstandard2.1")) {
                var assembly = projectItemWrapper.GetType().Assembly;
                var ns = $"{typeof(ModelEditorRunner).Namespace}.WinDesktop.";
                var resources = assembly.GetManifestResourceNames().Where(s => s.StartsWith(ns));

                
                foreach (var resource in resources) {
                    var path = $"{Path.GetFullPath($"{projectItemWrapper.FullPath}{projectItemWrapper.OutputPath}")}\\{resource.Replace(ns,"")}";
                    if (File.Exists(path)) {
                        File.Delete(path);
                    }

                    if (Path.GetExtension(path) == ".exe") {
                        mePath = path;
                    }
                    File.WriteAllBytes(path, Bytes(assembly.GetManifestResourceStream(resource)));
                }
            }
            else {
                mePath = GridHelper.ExtractME();
                
            }
            StartMEProcess(projectItemWrapper, assemblyPath, mePath);
        }

        static byte[] Bytes(Stream stream){
            if (stream is MemoryStream memoryStream){
                return memoryStream.ToArray();
            }

            using MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        void StartMEProcess(ProjectItemWrapper projectItemWrapper, string assemblyPath, string mePath) {
            try{
                var fullPath = projectItemWrapper.FullPath;
                var destFileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath) + "", Path.GetFileName(mePath) + ""));
                KillProcess(destFileName);
                if (!string.Equals(mePath, destFileName, StringComparison.OrdinalIgnoreCase)) {
                    File.Copy(mePath, destFileName,true);
                    var configPath = Path.Combine(Path.GetDirectoryName(        mePath)+"",Path.GetFileName(mePath)+".config");
                    if (File.Exists(configPath)) {
                        _dte.WriteToOutput("Copying App.config");
                        File.Copy(configPath,Path.Combine(Path.GetDirectoryName(destFileName)+"",Path.GetFileName(configPath)),true);
                    }
                }
                StartME(projectItemWrapper, assemblyPath, fullPath, destFileName);
            }
            catch (Exception e){
                MessageBox.Show(e.ToString());
            }
        }

        private void StartME(ProjectItemWrapper projectItemWrapper, string assemblyPath, string fullPath, string destFileName) {
            string debugMe = OptionClass.Instance.DebugME ? "d" : null;
            string arguments = String.Format("{0} {4} \"{1}\" \"{3}\" \"{2}\"", debugMe, Path.GetFullPath(assemblyPath),
                fullPath, projectItemWrapper.LocalPath, projectItemWrapper.IsApplicationProject);
            if (File.Exists(destFileName))
                try {
                    _dte.WriteToOutput($"Starting {destFileName} with arguments {arguments}");
                    Process.Start(destFileName, arguments);
                }
                catch (IOException) {
                    MessageBox.Show(
                        @"You have probably open the same model from another ME instance. If not please report this with reproduction details in eXpandFramework bugs forum");
                }
            else
                MessageBox.Show($@"Model editor not found at {destFileName}");
        }

        public void KillProcess(string path){
            const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using var searcher = new ManagementObjectSearcher(wmiQueryString);
            using var results = searcher.Get();
            var query = Process.GetProcesses()
                .Join(results.Cast<ManagementObject>(), p => p.Id, mo => (int) (uint) mo["ProcessId"],
                    (p, mo) => new{
                        Process = p,
                        Path = (string) mo["ExecutablePath"],
                        CommandLine = (string) mo["CommandLine"],
                    });
            foreach (var item in query.Where(arg => arg.Path==path)) {
                item.Process.Kill();
            }
        }

    }
}