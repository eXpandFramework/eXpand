using System;
using System.Collections.Generic;
using System.Configuration;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Workflow.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Workflow {
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
        private WorkflowServer server;
        private AppDomain domain;
        public void Start(string connectionString, string applicationName, List<ModuleBase> moduleBases) {
            try {
                domain = AppDomain.CreateDomain("ServerDomain");
                starter = (WorkflowServerStarter)domain.CreateInstanceAndUnwrap(
                    System.Reflection.Assembly.GetEntryAssembly().FullName, typeof(WorkflowServerStarter).FullName + "");
                starter.Start_(connectionString, applicationName, moduleBases);
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
        private void Start_(string connectionString, string applicationName, IEnumerable<ModuleBase> moduleBases) {
            var serverApplication = new ServerApplication();
            moduleBases.Each(@base => serverApplication.Modules.Add(@base));
            serverApplication.ApplicationName = applicationName;
            serverApplication.ConnectionString = connectionString;
            Type securityType = typeof(SecurityComplex).MakeGenericType(new[] { SecuritySystem.UserType, ((ISecurityComplex)SecuritySystem.Instance).RoleType });
            serverApplication.Security = (ISecurity)Activator.CreateInstance(securityType, new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            //            serverApplication.Security = new SecurityComplex<User, Role>(
            //                new WorkflowServerAuthentication(new BinaryOperator("UserName", "WorkflowService")));
            serverApplication.Setup();
            serverApplication.Logon();

            IObjectSpaceProvider objectSpaceProvider = serverApplication.ObjectSpaceProvider;

            server = new WorkflowServer(ConfigurationManager.AppSettings["WorkflowServerUrl"], objectSpaceProvider, objectSpaceProvider);
            server.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e) {
                e.WorkflowInstanceStoreBehavior.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
            };
            server.WorkflowDefinitionProvider = new WorkflowVersionedDefinitionProvider<XpoWorkflowDefinition, XpoUserActivityVersion>(objectSpaceProvider, null);
            server.StartWorkflowListenerService.DelayPeriod = TimeSpan.FromSeconds(5);
            server.StartWorkflowByRequestService.RequestsDetectionPeriod = TimeSpan.FromSeconds(5);
            server.RefreshWorkflowDefinitionsService.DelayPeriod = TimeSpan.FromSeconds(30);

            server.Start();
            server.CustomHandleException += delegate(object sender, DevExpress.ExpressApp.Workflow.ServiceModel.CustomHandleServiceExceptionEventArgs e) {
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
    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            Message = message;
        }
        public string Message { get; private set; }
    }

}