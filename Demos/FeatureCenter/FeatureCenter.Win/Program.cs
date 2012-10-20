using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp;
using Xpand.ExpressApp.Core;
using Xpand.ExpressApp.Workflow;

namespace FeatureCenter.Win {

    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] cmdargs) {

#if EASYTEST
			DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EditModelPermission.AlwaysGranted = Debugger.IsAttached;
            var winApplication = new FeatureCenterWindowsFormsApplication();
#if EASYTEST
			if(ConfigurationManager.ConnectionStrings["EasyTestConnectionString"] != null) {
				winApplication.ConnectionString = ConfigurationManager.ConnectionStrings["EasyTestConnectionString"].ConnectionString;
			}
#endif

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
                (winApplication).ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            }
            try {

                //                WorkflowServerStarter starter = null;
                //                winApplication.LoggedOn += delegate {
                //                    if (starter == null) {
                //                        starter = new WorkflowServerStarter();
                //                        starter.OnCustomHandleException += (sender1, args1) => MessageBox.Show(args1.Message);
                //
                //                        starter.Start(winApplication.ConnectionString, winApplication.ApplicationName);
                //                    }
                //                };

                winApplication.Setup();
                winApplication.LoggingOn += (sender, args) => {
                    if (cmdargs.Length > 0)
                        ((ModelApplicationBase)winApplication.Model).RemoveLayer(cmdargs[0]);
                };
                winApplication.Start();
            } catch (Exception e) {
                winApplication.HandleException(e);
            }
        }


    }


}
