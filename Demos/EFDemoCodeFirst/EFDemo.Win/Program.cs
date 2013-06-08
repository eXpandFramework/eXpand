using System;
using System.Reflection;
using System.Configuration;
using System.Globalization;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.EasyTest;
using EFDemo.Module;

namespace EFDemo.Win {
	public class Program {
#if CodeFirst
		private static Boolean IsCodeFirstMessageShown;
		private static Assembly CurrentDomain_AssemblyResolve(Object sender, ResolveEventArgs args) {
			if(args.Name.Contains(Consts.EntityFrameworkAssemblyName + ",") && !IsCodeFirstMessageShown) {
				IsCodeFirstMessageShown = true;
				String text = String.Format(@"Could not load assembly ""{0}"".", args.Name) + "\r\n\r\n" + Consts.CodeFirstMessageText + "\r\n";
				ExceptionDialogForm.ShowMessage("Error", text);
			}
			return null;
		}
#endif
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
#if CodeFirst
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
#endif

			EFDemoWinApplication winApplication = new EFDemoWinApplication();
#if DEBUG
			EasyTestRemotingRegistration.Register();
#endif
			winApplication.CustomizeFormattingCulture += new EventHandler<CustomizeFormattingCultureEventArgs>(winApplication_CustomizeFormattingCulture);
			winApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(winApplication_LastLogonParametersReading);
			try {
				if(ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
					winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
				}
				winApplication.Setup();
				winApplication.Start();
			}
			catch(Exception e) {
				winApplication.HandleException(e);
			}
		}
	}
}
