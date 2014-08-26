using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace DXPdbJunctions{
    internal class ApplicationWorkflow{
        private static ApplicationWorkflow _instance;
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool RemoveDirectory(string lpPathName);

        public static ApplicationWorkflow Instance {
            get { return _instance ?? (_instance = new ApplicationWorkflow()); }
        }

        public void CreateProcess(DXperience dXperience){
            ExtractJunctionExe();
            try{
                CreateJunction(dXperience);
            }
            finally{
                if (File.Exists("junction.exe"))
                    File.Delete("junction.exe");    
            }
        }

        private void ExtractJunctionExe(){
            if (File.Exists("junction.exe"))
                File.Delete("junction.exe");
            byte[] bytes;
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), "junction")){
                Debug.Assert(stream != null, "stream != null");
                bytes = new byte[(int) stream.Length];
                stream.Read(bytes, 0, bytes.Length);
            }
            File.WriteAllBytes("junction.exe", bytes);
        }


        private void CreateJunction(DXperience dXperience){
            var targetDir = dXperience.SourceFolder;

            var path = string.Format(@"C:\Projects\{0}", dXperience.Version);
            if (Directory.Exists(path )){
                Delete(path);
            }
            var junctionDir = string.Format(@"C:\Projects\{0}\BuildLabel\Temp\NetStudio.v{0}.2005",dXperience.Version);
            Directory.CreateDirectory(junctionDir);
            Directory.Delete(junctionDir);

            CreateJunction(junctionDir, targetDir);

            var winJunctionDir = junctionDir + @"\Win";
            if (Directory.Exists(winJunctionDir))
                Delete(winJunctionDir);
            CreateJunction(winJunctionDir, targetDir);

            var xJunctionDir = junctionDir + @"\X";
            if (Directory.Exists(xJunctionDir))
                Delete(xJunctionDir);
            Directory.CreateDirectory(xJunctionDir);

            var xafDirs = new[]{
                "DevExpress.ExpressApp", "DevExpress.ExpressApp.Workflow", "DevExpress.ExpressApp.Design",
                "DevExpress.ExpressApp.Tools", "DevExpress.Persistent"
            };
            foreach (var xafDir in xafDirs){
                CreateJunction(xJunctionDir + @"\" + xafDir, targetDir + @"\" + xafDir);
            }

            var targetModuleDir = targetDir + @"\DevExpress.ExpressApp.Modules";
            CreateJunction(xJunctionDir + @"\DevExpress.ExpressApp.Modules",targetModuleDir);

            var easyTestJunctionDir = xJunctionDir+@"\DevExpress.ExpressApp.EasyTest";
            if (Directory.Exists(easyTestJunctionDir))
                Delete(easyTestJunctionDir);
            Directory.CreateDirectory(easyTestJunctionDir);
            CreateJunction(easyTestJunctionDir + @"\DevExpress.ExpressApp.EasyTest.WinAdapter", targetModuleDir + @"\DevExpress.ExpressApp.EasyTest.WinAdapter");
            CreateJunction(easyTestJunctionDir + @"\DevExpress.ExpressApp.EasyTest.WebAdapter", targetModuleDir + @"\DevExpress.ExpressApp.EasyTest.WebAdapter");

            var aspJunctionDir = junctionDir+@"\ASP";
            if (Directory.Exists(aspJunctionDir))
                Delete(aspJunctionDir);
            CreateJunction(aspJunctionDir, targetDir);

        }

        private static void Delete(string path){
            Retry.Do(() =>{
                try{
                    Directory.Delete(path, true);
                }
                catch (IOException){
                }
            }, TimeSpan.FromSeconds(1), 5);
        }

        private void CreateJunction(string junctionDir, string targetDir){
            var processStartInfo = new ProcessStartInfo("junction.exe"){
                Arguments = @" -s """ + junctionDir.TrimEnd(Convert.ToChar(@"\")) + @""" """ + targetDir.TrimEnd(Convert.ToChar(@"\")) + @"""",
                UseShellExecute = false,RedirectStandardOutput = true,CreateNoWindow = true
            };
            var process = Process.Start(processStartInfo);
            Debug.Assert(process != null, "process != null");
            var text = process.StandardOutput.ReadToEnd();
            text = Regex.Replace(text, @"(.* www.sysinternals.com\r\n)(.*)", "$2",RegexOptions.Singleline | RegexOptions.IgnoreCase).Trim();
            Logger.Instance.AddText(text, text.Contains("Error")?TextItemStatus.Error : TextItemStatus.Success);
            process.WaitForExit();
            
        }
    }
}