using System;
using System.Configuration;
using System.Windows.Forms;

namespace DCSecurityDemo.UiLevel.Win {
    static class Program {
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] arguments) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DCSecurityDemoWindowsFormsApplication application = new DCSecurityDemoWindowsFormsApplication();
            try {
                if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                    application.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                }
                application.Setup();
                application.Start();
            }
            catch(Exception e) {
                application.HandleException(e);
            }
        }
    }
}
