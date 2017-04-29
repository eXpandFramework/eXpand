using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using EnvDTE80;
using Xpand.VSIX.Extensions;
using Xpand.VSIX.Options;
using Process = System.Diagnostics.Process;

namespace Xpand.VSIX.ModelEditor {
    public class ModelEditorRunner {
        private readonly DTE2 _dte = DteExtensions.DTE;
        public void Start(ProjectItemWrapper projectItemWrapper) {
            string outputFileName = projectItemWrapper.OutputFileName;
            
            string path = GetPath();
            if (path != null) StartMEProcess(projectItemWrapper, outputFileName, path);
        }

        private string GetPath(){
            var mePaths = OptionClass.Instance.MEs.Where(me => File.Exists(me.Path)).ToArray();
            foreach (var me in mePaths){
                var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(me.Path);
                var versionMatch = assembly.VersionMatch();
                _dte.WriteToOutput($"{me.Path} VersionMatch={versionMatch}");
                if (versionMatch)
                    return me.Path;
            }
            
            if (!mePaths.Any()){
                _dte.WriteToOutput("Use setting to add at least one model editor path for each major version");
                return null;
            }
            

            var versionMissMatchPaths = mePaths.Where(me => File.Exists(me.Path)).Select(me => me.Path).ToArray();
            var versionMissMatchMessage = versionMissMatchPaths.Any()
                ? "Version missmatch for:" + Environment.NewLine +
                  string.Join(Environment.NewLine, versionMissMatchPaths) + Environment.NewLine
                : null;
            var fileNotFoundPaths = mePaths.Where(me => !File.Exists(me.Path)).Select(me => me.Path).ToArray();
            var fileNotFoundMessage = fileNotFoundPaths.Any()
                ? "File not found:" + Environment.NewLine + string.Join(Environment.NewLine, fileNotFoundPaths)
                : null;
            _dte.WriteToOutput(versionMissMatchMessage + fileNotFoundMessage);
            return null;
        }

        void StartMEProcess(ProjectItemWrapper projectItemWrapper, string outputFileName, string path) {
            try{
                var fullPath = projectItemWrapper.FullPath;
                string assemblyPath = Path.Combine(fullPath, Path.Combine(projectItemWrapper.OutputPath, outputFileName));
                if (!File.Exists(assemblyPath)) {
                    MessageBox.Show($@"Assembly {assemblyPath} not found", null, MessageBoxButtons.OK);
                    return;
                }
            
                var destFileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(assemblyPath) + "", Path.GetFileName(path) + ""));
                KillProcess(destFileName);
                if (path.ToLower()!=destFileName.ToLower()){
                    File.Copy(path, destFileName,true);
                    var configPath = Path.Combine(Path.GetDirectoryName(path)+"",Path.GetFileName(path)+".config");
                    if (File.Exists(configPath))
                        File.Copy(configPath,Path.Combine(Path.GetDirectoryName(destFileName)+"",Path.GetFileName(configPath)),true);
                }
                string debugMe = OptionClass.Instance.DebugME ? "d":null;
                string arguments = String.Format("{0} {4} \"{1}\" \"{3}\" \"{2}\"", debugMe,Path.GetFullPath(assemblyPath), fullPath, projectItemWrapper.LocalPath,projectItemWrapper.IsApplicationProject);
                if (File.Exists(destFileName))
                    try{
                        Process.Start(destFileName, arguments);
                    }
                    catch (IOException){
                        MessageBox.Show("You have probably open the same model from another ME instance. If not please report this with reproduction details in eXpandFramework bugs forum");
                    }
                else
                    MessageBox.Show($"Model editor not found at {destFileName}");
            }
            catch (Exception e){
                MessageBox.Show(e.ToString());
            }
        }

        public void KillProcess(string path){
            const string wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get()) {
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
}