using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace XpandAddIns.ModelEditor {
    public class ModelEditorRunner {
        public void Start(ProjectWrapper projectWrapper) {
            string outputFileName = projectWrapper.OutPutFileName;
            if (outputFileName.ToLower().EndsWith(".exe"))
                outputFileName += ".config";
            
            string path = Options.ReadString(Options.ModelEditorPath);
            if (!String.IsNullOrEmpty(path)) {
                StartMEProcess(projectWrapper, outputFileName, path);
                return;
            }
            const string modeleditorpathPathIsEmpty = "ModelEditorPath path is empty";
            MessageBox.Show(modeleditorpathPathIsEmpty);
            
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
                if (path!=destFileName)
                    File.Copy(path, destFileName,true);
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