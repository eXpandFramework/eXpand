using System;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using DevExpress.ExpressApp.Workflow.Versioning;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.Workflow{
    public  abstract class WorkflowServerStarter : MarshalByRefObject {
        private class ServerApplication : XafApplication {
            protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
                args.ObjectSpaceProvider = new XPObjectSpaceProvider(args.ConnectionString, args.Connection, true);
            }
            protected override LayoutManager CreateLayoutManagerCore(bool simple) {
                throw new NotImplementedException();
            }
            public void Logon() {
                base.Logon(null);
            }
        }
        private static WorkflowServerStarter _starter;
        private WorkflowServer _server;
        private AppDomain _domain;
        void starter_OnCustomHandleException_(object sender, ExceptionEventArgs e) {
            if (OnCustomHandleException != null) {
                OnCustomHandleException(null, e);
            }
        }
        private void Start_<TWorkflowDefinition, TUserActivityVersion,TModulesProvider>(string connectionString, string applicationName, string url)
            where TWorkflowDefinition : IWorkflowDefinitionSettings
            where TUserActivityVersion : IUserActivityVersionBase where TModulesProvider:ModuleBase{
            ServerApplication serverApplication = new ServerApplication{ApplicationName = applicationName};
            serverApplication.Modules.Add(Activator.CreateInstance<TModulesProvider>());
            
            serverApplication.ConnectionString = connectionString;
            serverApplication.Setup();
            serverApplication.Logon();

            var objectSpaceProvider = serverApplication.ObjectSpaceProvider;
            _server = new WorkflowServer(url, objectSpaceProvider, objectSpaceProvider){
                WorkflowDefinitionProvider =
                    new WorkflowVersionedDefinitionProvider<TWorkflowDefinition, TUserActivityVersion>(
                        objectSpaceProvider, null),
                StartWorkflowListenerService ={DelayPeriod = TimeSpan.FromSeconds(5)},
                StartWorkflowByRequestService ={DelayPeriod = TimeSpan.FromSeconds(5)},
                RefreshWorkflowDefinitionsService ={DelayPeriod = TimeSpan.FromSeconds(600)}
            };
            _server.CustomizeHost += delegate(object sender, CustomizeHostEventArgs e) {
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

            _server.CustomHandleException += delegate(object sender, CustomHandleServiceExceptionEventArgs e) {
                Tracing.Tracer.LogError(e.Exception);
                if (OnCustomHandleException_ != null) {
                    OnCustomHandleException_(this, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Exception.Message + "\r\n\r\n'" + e.Service.GetType() + "' service"));
                }
                e.Handled = true;
            };
            _server.Start();
        }
        private void Stop_() {
            _server.Stop();
        }
        public void Start<TWorkflowDefinition, TUserActivityVersion, TModuleProvider>(string connectionString, string applicationName,  string url)
            where TWorkflowDefinition : IWorkflowDefinitionSettings
            where TUserActivityVersion : IUserActivityVersionBase 
            where TModuleProvider:ModuleBase{
            try {
                _domain = AppDomain.CreateDomain("ServerDomain");
                _starter = (WorkflowServerStarter)_domain.CreateInstanceAndUnwrap(
                    Assembly.GetEntryAssembly().FullName, GetType().FullName);
                _starter.OnCustomHandleException_ += starter_OnCustomHandleException_;
                _starter.Start_<TWorkflowDefinition,TUserActivityVersion,TModuleProvider>(connectionString, applicationName,url);
            }
            catch (Exception e) {
                Tracing.Tracer.LogError(e);
                if (OnCustomHandleException != null) {
                    OnCustomHandleException(null, new ExceptionEventArgs("Exception occurs:\r\n\r\n" + e.Message));
                }
            }
        }
        public void Stop() {
            if (_starter != null) {
                _starter.Stop_();
            }
            if (_domain != null) {
                AppDomain.Unload(_domain);
            }
        }
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException_;
        public event EventHandler<ExceptionEventArgs> OnCustomHandleException;

        public void Start<TWorkflowDefinition, TUserActivityVersion, TModuleProvider>(XafApplication application, string url = "http://localhost:46232")
            where TWorkflowDefinition : IWorkflowDefinitionSettings
            where TUserActivityVersion : IUserActivityVersionBase 
            where  TModuleProvider:ModuleBase{
                Start<TWorkflowDefinition, TUserActivityVersion,TModuleProvider>(application.ConnectionString, application.ApplicationName, url);
        }
    }
    [Serializable]
    public class ExceptionEventArgs : EventArgs {
        public ExceptionEventArgs(string message) {
            Message = message;
        }
        public string Message { get; private set; }
    }

}
