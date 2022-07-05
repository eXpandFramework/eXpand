using System;
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using Fasterflect;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Workflow{
    public  abstract class WorkflowServerStarter : MarshalByRefObject {
        public class ServerApplication : XafApplication {
            public ServerApplication(){
                DatabaseVersionMismatch+=OnDatabaseVersionMismatch;
            }

            private void OnDatabaseVersionMismatch(object o, DatabaseVersionMismatchEventArgs e){
                e.Updater.Update();
            }

            protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
                args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
            }
            protected override LayoutManager CreateLayoutManagerCore(bool simple) {
                throw new NotImplementedException();
            }
        }
        private static WorkflowServerStarter _starter;
        private XpandWorkflowServer _server;
        private AppDomain _domain;
        void StarterOnOnServerDomainCustomHandleException(object sender, ExceptionEventArgs e){
            OnCustomHandleException?.Invoke(null, e);
        }

        private void StartOnServerDomain<TModulesProvider>(string connectionString, string applicationName) where TModulesProvider:ModuleBase{
            var serverApplication = GetServerApplication();
            serverApplication.Modules.Add(Activator.CreateInstance<TModulesProvider>());
            serverApplication.ApplicationName = applicationName;

            serverApplication.ConnectionString = connectionString;
            serverApplication.Setup();
            serverApplication.Logon();

            var objectSpaceProvider = serverApplication.ObjectSpaceProvider;
            var xpoUserActivityVersionType = XpandModuleBase.GetDxBaseImplType("DevExpress.ExpressApp.Workflow.Versioning.XpoUserActivityVersion");
            var engine=(WorkflowVersioningEngine) typeof(PersistentWorkflowVersioningEngine<>).MakeGenericType(xpoUserActivityVersionType).CreateInstance(objectSpaceProvider);
            var workflowDefinitionProvider = (IWorkflowDefinitionProvider)typeof(XpandWorkflowDefinitionProvider<>).MakeGenericType(xpoUserActivityVersionType).CreateInstance(XpandWorkFlowModule.WorkflowTypes.ToList(),engine);
            var xpandWorkflowServer = new XpandWorkflowServer("http://localhost:46232", workflowDefinitionProvider, objectSpaceProvider);
            xpandWorkflowServer.CustomizeHost += delegate (object sender, CustomizeHostEventArgs e) {
                // NOTE: Uncomment this section to use alternative workflow configuration.
                //
                // SqlWorkflowInstanceStoreBehavior
                //
                //e.WorkflowInstanceStoreBehavior = null;
                //System.ServiceModel.Activities.Description.SqlWorkflowInstanceStoreBehavior sqlWorkflowInstanceStoreBehavior = new System.ServiceModel.Activities.Description.SqlWorkflowInstanceStoreBehavior("Integrated Security=SSPI;Pooling=false;Data Source=(local);Initial Catalog=WorkflowsStore");
                //sqlWorkflowInstanceStoreBehavior.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
                //e.Host.Description.Behaviors.Add(sqlWorkflowInstanceStoreBehavior);
                //e.WorkflowIdleBehavior.TimeToPersist = TimeSpan.FromSeconds(1);
                //e.WorkflowIdleBehavior.TimeToPersist = TimeSpan.FromSeconds(10);
                //e.WorkflowIdleBehavior.TimeToUnload = TimeSpan.FromSeconds(10);
                e.WorkflowInstanceStoreBehavior.WorkflowInstanceStore.RunnableInstancesDetectionPeriod = TimeSpan.FromSeconds(2);
            };
            _server= xpandWorkflowServer;
            _server.CustomHandleException += delegate(object sender, CustomHandleServiceExceptionEventArgs e) {
                Tracing.Tracer.LogError(e.Exception);
                OnServerDomainCustomHandleException?.Invoke(this, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Exception.Message + "\r\n\r\n'" + e.Service.GetType() + "' service"));
                e.Handled = true;
            };
            _server.Start();
        }

        protected virtual ServerApplication GetServerApplication(){
            return new ServerApplication();
        }

        private void Stop_() {
            _server.Stop();
        }
        public void Start<TModuleProvider>(string connectionString, string applicationName) where TModuleProvider:ModuleBase{
            try {
                _domain = AppDomain.CreateDomain("Server");
                _starter = (WorkflowServerStarter)_domain.CreateInstanceAndUnwrap(
                    Assembly.GetEntryAssembly().FullName, GetType().FullName ?? throw new InvalidOperationException());
                _starter.OnServerDomainCustomHandleException += StarterOnOnServerDomainCustomHandleException;
                var workflowServerEventArgs = new WorkflowServerEventArgs();
                OnWorkflowServerRequested(workflowServerEventArgs);
                _starter.StartOnServerDomain<TModuleProvider>(connectionString, applicationName);
            }
            catch (Exception e) {
                Tracing.Tracer.LogError(e);
                OnCustomHandleException?.Invoke(null, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Message));
            }
        }
        public void Stop() {
            _starter?.Stop_();
            if (_domain != null) {
                AppDomain.Unload(_domain);
            }
        }
        event EventHandler<ExceptionEventArgs> OnServerDomainCustomHandleException;
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException;
        public event EventHandler<WorkflowServerEventArgs> WorkflowServerRequested;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "XAF0013:Avoid reading the XafApplication.ConnectionString property", Justification = "<Pending>")]
        public void Start<TModuleProvider>(XafApplication application) where  TModuleProvider:ModuleBase{
                Start<TModuleProvider>(application.ConnectionString, application.ApplicationName);
        }

        protected virtual void OnWorkflowServerRequested(WorkflowServerEventArgs e){
            WorkflowServerRequested?.Invoke(this, e);
        }
    }

    public class WorkflowServerEventArgs : EventArgs{
        
    }

    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            Message = message;
        }
        public string Message { get; }
    }

}
