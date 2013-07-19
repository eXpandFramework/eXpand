using System;
using System.Configuration;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Workflow.Win;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using WorkflowDemo.Module;
using WorkflowDemo.Module.Activities;
using WorkflowDemo.Module.Win;
using Xpand.ExpressApp.Workflow;

namespace WorkflowDemo.Win
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
		[STAThread]
        static void Main(string[] arguments)
        {
            
            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			EditModelPermission.AlwaysGranted = System.Diagnostics.Debugger.IsAttached;
			WorkflowDemoWindowsFormsApplication xafApplication = new WorkflowDemoWindowsFormsApplication();

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
            xafApplication.Modules.FindModule<WorkflowWindowsFormsModule>().QueryAvailableActivities += delegate(object sender, ActivitiesInformationEventArgs e) {
                e.ActivitiesInformation.Add(new ActivityInformation(typeof(CreateTask), "Code Activities", "Create Task", ImageLoader.Instance.GetImageInfo("CreateTask").Image));
			};

            xafApplication.LastLogonParametersReading += new EventHandler<LastLogonParametersReadingEventArgs>(xafApplication_LastLogonParametersReading);
            WorkflowServerStarter starter = null;
            xafApplication.LoggedOn += delegate(object sender, LogonEventArgs e) {
                if(starter == null) {
                    starter = new WorkflowServerStarter();
                    starter.OnCustomHandleException += delegate(object sender1, ExceptionEventArgs args1) {
                        MessageBox.Show(args1.Message);
                    };

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

    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            this.Message = message;
        }
        public string Message { get; private set; }
    }

    public class WorkflowServerStarter : MarshalByRefObject {
        private class ServerApplication : XpandWorkflowApplication {

        }
        private static WorkflowServerStarter starter;
        private WorkflowServer server;
        private AppDomain domain;
        void starter_OnCustomHandleException_(object sender, ExceptionEventArgs e) {
            if(OnCustomHandleException != null) {
                OnCustomHandleException(null, e);
            }
        }
        private void Start_(string connectionString, string applicationName) {
            ServerApplication serverApplication = new ServerApplication();
            serverApplication.Modules.Add(new WorkflowDemoWindowsFormsModule());
            serverApplication.ApplicationName = applicationName;
            serverApplication.ConnectionString = connectionString;
            serverApplication.Security = new SecurityComplex<User, Role>(
                new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            serverApplication.Setup();
            serverApplication.Logon();

            IObjectSpaceProvider objectSpaceProvider = serverApplication.ObjectSpaceProvider;

            server = new XpandWorkflowServer("http://localhost:46232", objectSpaceProvider, objectSpaceProvider);
            server.WorkflowDefinitionProvider = new WorkflowVersionedDefinitionProvider<XpoWorkflowDefinition, XpoUserActivityVersion>(objectSpaceProvider, null);
            server.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(5);
            server.StartWorkflowByRequestService.DelayPeriod = TimeSpan.FromSeconds(5);
            server.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromSeconds(600);
            server.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e) {
                //
                // SqlWorkflowInstanceStoreBehavior
                //
                //e.WorkflowInstanceStoreBehavior = null;
                //System.ServiceModel.Activities.Description.SqlWorkflowInstanceStoreBehavior sqlWorkflowInstanceStoreBehavior = new System.ServiceModel.Activities.Description.SqlWorkflowInstanceStoreBehavior("Integrated Security=SSPI;Pooling=false;Data Source=(local);Initial Catalog=WorkflowsStore");
                //sqlWorkflowInstanceStoreBehavior.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
                //e.Host.Description.Behaviors.Add(sqlWorkflowInstanceStoreBehavior);
                //e.WorkflowIdleBehavior.TimeToPersist = TimeSpan.FromSeconds(1);
                e.WorkflowInstanceStoreBehavior.WorkflowInstanceStore.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
            };

            server.CustomHandleException += delegate(object sender, CustomHandleServiceExceptionEventArgs e) {
                Tracing.Tracer.LogError(e.Exception);
                if(OnCustomHandleException_ != null) {
                    OnCustomHandleException_(this, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Exception.Message + "\r\n\r\n'" + e.Service.GetType() + "' service"));
                }
                e.Handled = true;
            };
            server.Start();
        }
        private void Stop_() {
            server.Stop();
        }
        public void Start(string connectionString, string applicationName) {
            try {
                domain = AppDomain.CreateDomain("ServerDomain");
                starter = (WorkflowServerStarter)domain.CreateInstanceAndUnwrap(
                    System.Reflection.Assembly.GetEntryAssembly().FullName, typeof(WorkflowServerStarter).FullName);
				starter.OnCustomHandleException_ += new EventHandler<ExceptionEventArgs>(starter_OnCustomHandleException_);
                starter.Start_(connectionString, applicationName);
            }
            catch(Exception e) {
                Tracing.Tracer.LogError(e);
                if(OnCustomHandleException != null) {
                    OnCustomHandleException(null, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Message));
                }
            }
        }
        public void Stop() {
            if(starter != null) {
                starter.Stop_();
            }
            if(domain != null) {
                AppDomain.Unload(domain);
            }
        }
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException_;
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException;
    }
}
