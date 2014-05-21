using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.Persistent.Base;

namespace WorldCreatorTester.Win {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            string[] configs = Directory.GetFiles(@"C:\eXpandFrameWork\eXpand\Demos", "config.xml",
                SearchOption.AllDirectories);
            foreach (string path in configs){
                var newFile = new StringBuilder();

                string temp = "";

                string[] file = File.ReadAllLines(path);

                for (int i = 0; i < file.Length; i++){
                    string line = file[i];
                    if (line.Contains(@"Url=""http://localhost:4032""")){
                        temp = line.Replace(@"Url=""http://localhost:4032""", @"Url=""http://localhost:500" + i + @"""");

                        newFile.Append(temp + "\r\n");

                        continue;
                    }

                    newFile.Append(line + "\r\n");
                }

                File.WriteAllText(path, newFile.ToString());
            }

           
#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = true;
            var winApplication = new WorldCreatorTesterWindowsFormsApplication();
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null){
                winApplication.ConnectionString =
                    ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            try{
                winApplication.Setup();
                winApplication.Start();
            }
            catch (Exception e){
                winApplication.HandleException(e);
            }
        }
    }
}
