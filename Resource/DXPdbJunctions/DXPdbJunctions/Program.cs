using System;
using System.Windows.Forms;

namespace DXPdbJunctions {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm.Instance.Run();
        }
    }
}
