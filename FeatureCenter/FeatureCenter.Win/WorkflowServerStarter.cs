using System;
using System.Collections.Generic;
using System.Configuration;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.BaseImpl;
using Xpand.ExpressApp.Workflow;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace FeatureCenter.Win {


    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            Message = message;
        }
        public string Message { get; private set; }
    }

    public class WorkflowServerStarter : MarshalByRefObject {
        private class ServerApplication : XafApplication {
            protected override DevExpress.ExpressApp.Layout.LayoutManager CreateLayoutManagerCore(bool simple) {
                throw new NotImplementedException();
            }
            public void Logon() {
                base.Logon(null);
            }

        }
        private static WorkflowServerStarter starter;
        private XpandWorkflowServer server;
        private AppDomain domain;
        public void Start(string connectionString, string applicationName) {
            try {
                domain = AppDomain.CreateDomain("ServerDomain");
                starter = (WorkflowServerStarter)domain.CreateInstanceAndUnwrap(
                    System.Reflection.Assembly.GetEntryAssembly().FullName, typeof(WorkflowServerStarter).FullName + "");
                starter.Start_(connectionString, applicationName);
                starter.OnCustomHandleException_ += starter_OnCustomHandleException_;
            } catch (Exception e) {
                Tracing.Tracer.LogError(e);
                if (OnCustomHandleException != null) {
                    OnCustomHandleException(null, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Message));
                }
            }
        }

        void starter_OnCustomHandleException_(object sender, ExceptionEventArgs e) {
            if (OnCustomHandleException != null) {
                OnCustomHandleException(null, e);
            }
        }
        public void Stop() {
            if (domain != null) {
                AppDomain.Unload(domain);
            }
        }
        private void Start_(string connectionString, string applicationName) {
            var serverApplication = new ServerApplication();
            serverApplication.Modules.Add(new XpandWorkFlowModule());
            serverApplication.ApplicationName = applicationName;
            serverApplication.ConnectionString = connectionString;


            serverApplication.Security = new SecurityComplex<User, Role>(
                new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            serverApplication.Setup();
            serverApplication.Logon();

            IObjectSpaceProvider objectSpaceProvider = serverApplication.ObjectSpaceProvider;

            server = new XpandWorkflowServer(ConfigurationManager.AppSettings["WorkflowServerAddress"], objectSpaceProvider, objectSpaceProvider);
            server.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e) {
                e.WorkflowInstanceStoreBehavior.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
            };
            //            server.WorkflowDefinitionProvider = new WorkflowVersionedDefinitionProvider<XpoWorkflowDefinition, XpoUserActivityVersion>(objectSpaceProvider, null);
            server.WorkflowDefinitionProvider = new XpandWorkflowDefinitionProvider(typeof(XpoWorkflowDefinition), new List<Type> { typeof(ScheduledWorkflow), typeof(ObjectChangedWorkflow) });
            server.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(5);
            server.StartWorkflowByRequestService.RequestsDetectionPeriod = TimeSpan.FromSeconds(5);
            server.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromSeconds(30);

            server.Start();
            server.CustomHandleException += (sender, e) => {
                Tracing.Tracer.LogError(e.Exception);
                if (OnCustomHandleException_ != null) {
                    OnCustomHandleException_(this, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Exception.Message + "\r\n\r\n'" + e.Service.GetType() + "' service"));
                }
                e.Handled = true;

            };
        }
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException_;
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException;
    }
}
