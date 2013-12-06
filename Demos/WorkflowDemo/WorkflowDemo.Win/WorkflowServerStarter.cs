using System;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using WorkflowDemo.Module.Win;
using Xpand.ExpressApp.Workflow;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace WorkflowDemo.Win {
    public class WorkflowServerStarter : MarshalByRefObject {
        private static WorkflowServerStarter starter;
        private XpandWorkflowServer _xpandWorkflowServer;
        private AppDomain domain;
        void starter_OnCustomHandleException_(object sender, ExceptionEventArgs e) {
            if(OnCustomHandleException != null) {
                OnCustomHandleException(null, e);
            }
        }
        private void Start_(string connectionString, string applicationName) {
            var securityComplex = new SecurityComplex<User, Role>(new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            var serverApplication = new XpandWorkflowApplication(securityComplex);
            serverApplication.Modules.Add(new WorkflowDemoWindowsFormsModule());
            serverApplication.ApplicationName = applicationName;
            serverApplication.ConnectionString = connectionString;
            
            serverApplication.Security = securityComplex;
            serverApplication.Setup();
            serverApplication.Logon();

            IObjectSpaceProvider objectSpaceProvider = serverApplication.ObjectSpaceProvider;

            _xpandWorkflowServer = new XpandWorkflowServer("http://localhost:46232", objectSpaceProvider, objectSpaceProvider);
            _xpandWorkflowServer.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e) {
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
            _xpandWorkflowServer.WorkflowDefinitionProvider = new XpandWorkflowDefinitionProvider(typeof(XpoWorkflowDefinition), new List<Type> { typeof(ScheduledWorkflow), typeof(ObjectChangedWorkflow) });
            _xpandWorkflowServer.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(5);
            _xpandWorkflowServer.StartWorkflowByRequestService.DelayPeriod = TimeSpan.FromSeconds(5);
            _xpandWorkflowServer.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromSeconds(60);

            _xpandWorkflowServer.CustomHandleException += delegate(object sender, CustomHandleServiceExceptionEventArgs e) {
                Tracing.Tracer.LogError(e.Exception);
                if(OnCustomHandleException_ != null) {
                    OnCustomHandleException_(this, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Exception.Message + "\r\n\r\n'" + e.Service.GetType() + "' service"));
                }
                e.Handled = true;
            };
            _xpandWorkflowServer.Start();
        }
        private void Stop_() {
            _xpandWorkflowServer.Stop();
        }
        public void Start(string connectionString, string applicationName) {
            try {
                domain = AppDomain.CreateDomain("ServerDomain");
                starter = (WorkflowServerStarter)domain.CreateInstanceAndUnwrap(
                    System.Reflection.Assembly.GetEntryAssembly().FullName, typeof(WorkflowServerStarter).FullName);
                starter.OnCustomHandleException_ += starter_OnCustomHandleException_;
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
    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            Message = message;
        }
        public string Message { get; private set; }
    }

}