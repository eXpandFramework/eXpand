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
        ObjectChangedStartWorkflowService _objectChangedStartWorkflowService;
        ScheduledWorkflowStartService _scheduledWorkflowStartService;
        ObjectChangedWorkflowStartService _objectChangedWorkflowStartService;


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
            _startWorkflowOnObjectChangeService =
                new StartWorkflowOnObjectChangeService(TimeSpan.FromSeconds(15));
            _objectChangedStartWorkflowService = new ObjectChangedStartWorkflowService();
            _scheduledWorkflowStartService = new ScheduledWorkflowStartService();
            _objectChangedWorkflowStartService = new ObjectChangedWorkflowStartService();
        }

        void InitializeDefaults() {
            if (StartWorkflowOnObjectChangeService != null) {
                ServiceProvider.AddService(StartWorkflowOnObjectChangeService);
            }
            if (ObjectChangedStartWorkflowService != null) {
                ServiceProvider.AddService(ObjectChangedStartWorkflowService);
            }
            if (ScheduledWorkflowStartService != null) {
                ServiceProvider.AddService(ScheduledWorkflowStartService);
            }
            if (ObjectChangedWorkflowStartService != null) {
                ServiceProvider.AddService(ObjectChangedWorkflowStartService);
            }
        }

        public new void Start() {
            InitializeDefaults();
            base.Start();
        }
        public ObjectChangedWorkflowStartService ObjectChangedWorkflowStartService => _objectChangedWorkflowStartService;

        public ScheduledWorkflowStartService ScheduledWorkflowStartService => _scheduledWorkflowStartService;

        public StartWorkflowOnObjectChangeService StartWorkflowOnObjectChangeService => _startWorkflowOnObjectChangeService;

        public ObjectChangedStartWorkflowService ObjectChangedStartWorkflowService => _objectChangedStartWorkflowService;
    }
}