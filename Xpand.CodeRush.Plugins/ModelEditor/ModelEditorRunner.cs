using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using Xpand.CodeRush.Plugins.Extensions;
using Process = System.Diagnostics.Process;

namespace Xpand.CodeRush.Plugins.ModelEditor {
    public class ModelEditorRunner {
        private readonly DTE _dte = DevExpress.CodeRush.Core.CodeRush.ApplicationObject;
        public void Start(ProjectWrapper projectWrapper) {
            string outputFileName = projectWrapper.OutPutFileName;
            if (outputFileName.ToLower().EndsWith(".exe"))
                outputFileName += ".config";
            
            string path = GetPath();
            if (path != null) StartMEProcess(projectWrapper, outputFileName, path);
        }

        private string GetPath(){
            var dxVersion = DevExpress.CodeRush.Core.CodeRush.Solution.Active.GetDXVersion();
            var mePaths = Options.GetMEPaths();
            foreach (var me in mePaths.Where(me => File.Exists(me.Path))){
                var assembly = Assembly.ReflectionOnlyLoadFrom(me.Path);
                if (assembly.GetReferencedAssemblies().Any(name => name.Name.StartsWith("DevExpress") && name.Name.Contains(dxVersion)))
                    return me.Path;
            }
            
            if (!mePaths.Any()){
                _dte.WriteToOutput("Use setting to add at least one model editor path ");
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

        void StartMEProcess(ProjectWrapper projectWrapper, string outputFileName, string path) {
            try{
                var fullPath = projectWrapper.FullPath;
                string assemblyPath = Path.Combine(fullPath, Path.Combine(projectWrapper.OutputPath, outputFileName));
                if (!File.Exists(assemblyPath)) {
                    MessageBox.Show(String.Format(@"Assembly {0} not found", assemblyPath), null, MessageBoxButtons.OK);
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
                string debugMe = Options.ReadBool(Options.DebugME)?"d":null;
                string arguments = String.Format("{0} \"{1}\" \"{3}\" \"{2}\"", debugMe,Path.GetFullPath(assemblyPath), fullPath, projectWrapper.LocalPath);
                if (File.Exists(destFileName))
                    Process.Start(destFileName, arguments);
                else
                    MessageBox.Show(String.Format("Model editor not found at {0}", destFileName));
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