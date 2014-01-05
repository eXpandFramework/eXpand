using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.DemoData.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.EasyTest;

namespace EFDemo.Win {
    public class Program {
        private static void winApplication_CustomizeFormattingCulture(Object sender, CustomizeFormattingCultureEventArgs e) {
            e.FormattingCulture = CultureInfo.GetCultureInfo("en-US");
        }
        private static void winApplication_LastLogonParametersReading(Object sender, LastLogonParametersReadingEventArgs e) {
            if(String.IsNullOrWhiteSpace(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }

        [STAThread]
        public static void Main(string[] arguments) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EFDemoWinApplication winApplication = new EFDemoWinApplication();
#if DEBUG
            EasyTestRemotingRegistration.Register();
#endif
            winApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(winApplication_CustomizeFormattingCulture);
            winApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(winApplication_LastLogonParametersReading);
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["ConnectionString"];
            if(connectionStringSettings != null) {
                winApplication.ConnectionString = connectionStringSettings.ConnectionString;
            }
            else if(string.IsNullOrEmpty(winApplication.ConnectionString) && winApplication.Connection == null) {
                connectionStringSettings = ConfigurationManager.ConnectionStrings["SqlExpressConnectionString"];
                if(connectionStringSettings != null) {
                    winApplication.ConnectionString = DbEngineDetector.PatchConnectionString(connectionStringSettings.ConnectionString);
                }
            }
            try {
                winApplication.Setup();
                winApplication.Start();
            }
            catch(Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
