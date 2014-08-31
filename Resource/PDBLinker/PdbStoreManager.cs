using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PDBLinker{
    public interface IPdbStoreManager {
        PdbSrcSrvSection ReadSrcSrv(string pdbFilePath);
        string WriteSrcSrv(string pdbFilePath, PdbSrcSrvSection srcSrvSection);
    }

    public class PdbStoreManager :  IPdbStoreManager{
        private readonly string _debugToolsPath;

        public PdbStoreManager(string debugToolsPath){
            _debugToolsPath = debugToolsPath;
            if (!File.Exists(PdbStrPath()))
                throw new IOException(string.Format("File not exists ('{0}')", PdbStrPath()));
        }

        public PdbSrcSrvSection ReadSrcSrv(string pdbFilePath){
            throw new NotImplementedException();
        }

        public string WriteSrcSrv(string pdbFilePath, PdbSrcSrvSection srcSrvSection){
            string tempFile = Path.GetTempFileName();
            var currentDirectory = Environment.CurrentDirectory;
            try{
                File.WriteAllText(tempFile, srcSrvSection.ToString());
                Environment.CurrentDirectory = Path.GetDirectoryName(PdbStrPath())+"";
                var processStartInfo = new ProcessStartInfo{
                    FileName = Path.Combine(Environment.CurrentDirectory,"pdbstr.exe"),
                    Arguments =@"-w -p:""" +pdbFilePath + @""" -i:""" + tempFile+@""" -s:srcsrv",
                    UseShellExecute = false,CreateNoWindow = true,RedirectStandardOutput = true
                };
                using (var pdbstrProcess=new Process{StartInfo = processStartInfo}){
                    pdbstrProcess.Start();
                    string result = pdbstrProcess.StandardOutput.ReadToEnd();
                    pdbstrProcess.WaitForExit();
                    return result;
                }
            }
            finally{
                Environment.CurrentDirectory=currentDirectory;
                File.Delete(tempFile);
            }
        }

        private string PdbStrPath(){
            return Path.GetFullPath(Path.Combine(_debugToolsPath, "pdbstr.exe"));
        }

        public string[] GetIndexedFiles(string pdbFile){
            var currentDirectory = Environment.CurrentDirectory;
            try {
                Environment.CurrentDirectory = Path.GetDirectoryName(PdbStrPath()) + "";
                var processStartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(Environment.CurrentDirectory, "srctool.exe"),
                    Arguments = @"-r """ + pdbFile + @"""",
                    UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true
                };
                using (var pdbstrProcess = new Process { StartInfo = processStartInfo }) {
                    pdbstrProcess.Start();
                    string result = pdbstrProcess.StandardOutput.ReadToEnd();
                    pdbstrProcess.WaitForExit();
                    return result.Split(Environment.NewLine.ToCharArray()).Where(s => !string.IsNullOrEmpty(s)).TakeAllButLast().ToArray();
                }
            }
            finally {
                Environment.CurrentDirectory = currentDirectory;
            }
        }
    }
    
}