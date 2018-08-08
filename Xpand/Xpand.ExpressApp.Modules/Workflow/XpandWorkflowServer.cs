                                                                                                                             using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.CommonServices;
using DevExpress.ExpressApp.Workflow.Server;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace Xpand.ExpressApp.Workflow {
    public class XpandWorkflowServer : WorkflowServer {
        StartWorkflowOnObjectChangeService _startWorkflowOnObjectChangeService;
        ScheduledWorkflowStartService _scheduledWorkflowStartService;


        public XpandWorkflowServer(string baseUri, IWorkflowDefinitionProvider workflowDefinitionProvider,
            IObjectSpaceProvider objectSpaceProvider)
            : this(baseUri, workflowDefinitionProvider, new BasicHttpBinding(), objectSpaceProvider, objectSpaceProvider){
        }

        public XpandWorkflowServer(string baseUri, IWorkflowDefinitionProvider workflowDefinitionProvider, Binding binding, IObjectSpaceProvider servicesObjectSpaceProvider, IObjectSpaceProvider activitiesObjectSpaceProvider)
            : base(baseUri,binding,servicesObjectSpaceProvider,activitiesObjectSpaceProvider) {
            WorkflowDefinitionProvider=workflowDefinitionProvider;
            CreateServices();
        }

        void CreateServices(){
            _startWorkflowOnObjectChangeService = new StartWorkflowOnObjectChangeService(TimeSpan.FromSeconds(15));
            _scheduledWorkflowStartService = new ScheduledWorkflowStartService();
        }

        void InitializeDefaults() {
            if (StartWorkflowOnObjectChangeService != null) {
                ServiceProvider.AddService(StartWorkflowOnObjectChangeService);
            }
            if (ScheduledWorkflowStartService != null) {
                ServiceProvider.AddService(ScheduledWorkflowStartService);
            }
        }

        public new void Start() {
            InitializeDefaults();
            base.Start();
        }

        public ScheduledWorkflowStartService ScheduledWorkflowStartService => _scheduledWorkflowStartService;

        public StartWorkflowOnObjectChangeService StartWorkflowOnObjectChangeService => _startWorkflowOnObjectChangeService;

    }
}