using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using Xpand.ExpressApp.Security.Core;
using Xpand.ExpressApp.Security.Registration;

namespace EmailTester.Win {
    public class MyClass{
        // Fields...
        private string _UserName;

        public string UserName {
            get { return "aaaaaaaaaaaaaaaaaa"; }
            set {
                _UserName = value;
            }
        }
         
    }

//    public class RegisterUserParameters {
//        // Fields...
//        private object _User;
//
//        public object User {
//            get { return _User; }
//            set {
//                _User = value;
//            }
//        }
//         
//    }
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            


#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = true;
            var winApplication = new EmailTesterWindowsFormsApplication();
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif
            try {
                winApplication.UseOldTemplates=false;
                winApplication.Setup();
//                var razorEngineService = RazorEngineService.Create(new TemplateServiceConfiguration() { Debug = true });
//                var objectSpace = winApplication.CreateObjectSpace();
//                var xpandUser = objectSpace.FindObject<XpandUser>(CriteriaOperator.Parse(""));
//                var registerUserParameters = new RegisterUserParameters() { User = xpandUser };
//                            var runCompile = razorEngineService.RunCompile("eee @Model.User.UserName ", Guid.NewGuid().ToString(), null, registerUserParameters);
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }
    }
}
