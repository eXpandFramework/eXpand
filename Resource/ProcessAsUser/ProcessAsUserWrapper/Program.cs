using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ProcessAsUserWrapper {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var mainForm = new Form1();
            mainForm.Load+= (sender, eventArgs) =>{
                var processStartInfo = new ProcessStartInfo(args[0], args.Length == 2 ? args[1] : null) {UseShellExecute=false, RedirectStandardOutput = true,WindowStyle = ProcessWindowStyle.Minimized};
                var process = Process.Start(processStartInfo);
                
                Debug.Assert(process != null, "process != null");
                process.WaitForExit();
                var streamWriter = File.CreateText("processAsuserWrapper.log");
                streamWriter.Write(process.StandardOutput.ReadToEnd());
                streamWriter.Close();
                Application.ExitThread();
            };
            Application.Run(mainForm);
        }

    }
}
