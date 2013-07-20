using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.Win;
using WorkflowDemo.Module.Activities;

namespace WorkflowDemo.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
			var xafApplication = new WorkflowDemoWindowsFormsApplication();

            //
            // SqlWorkflowInstanceStoreBehavior
            //
            //WorkflowModule workflowModule = xafApplication.Modules.FindModule<WorkflowModule>();
            //workflowModule.WorkflowInstanceType = null;
            //workflowModule.WorkflowInstanceKeyType = null;
#if DEBUG
            DevExpress.ExpressApp.Win.EasyTest.EasyTestRemotingRegistration.Register();
#endif	
            if (ConfigurationManager.ConnectionStrings["ConnectionString"] != null) {
				xafApplication.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
			}
            xafApplication.Modules.FindModule<WorkflowWindowsFormsModule>().QueryAvailableActivities +=
                (sender, e) =>
                e.ActivitiesInformation.Add(new ActivityInformation(typeof (CreateTask), "Code Activities",
                                                                    "Create Task",
                                                                    ImageLoader.Instance.GetImageInfo("CreateTask")
                                                                               .Image));

            xafApplication.LastLogonParametersReading += xafApplication_LastLogonParametersReading;
            WorkflowServerStarter starter = null;
            xafApplication.LoggedOn += delegate {
                if(starter == null) {
                    starter = new WorkflowServerStarter();
                    starter.OnCustomHandleException += (sender1, args1) => MessageBox.Show(args1.Message);

                    starter.Start(xafApplication.ConnectionString, xafApplication.ApplicationName);
                }
            };

            try
            {
                xafApplication.Setup();
				xafApplication.Start();
            }
            catch (Exception e)
            {
				xafApplication.HandleException(e);
            }

            if(starter != null) {
                starter.Stop();
            }
        }

        static void xafApplication_LastLogonParametersReading(object sender, LastLogonParametersReadingEventArgs e) {
            if(string.IsNullOrEmpty(e.SettingsStorage.LoadOption("", "UserName"))) {
                e.SettingsStorage.SaveOption("", "UserName", "Sam");
            }
        }
    }

}
