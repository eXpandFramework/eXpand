using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Workflow.Server;
using Xpand.ExpressApp.Workflow.ObjectChangedWorkflows;
using Xpand.ExpressApp.Workflow.ScheduledWorkflows;

namespace Xpand.ExpressApp.Workflow {
    public class XpandWorkflowServer : WorkflowServer {
        StartWorkflowOnObjectChangeService _startWorkflowOnObjectChangeService;
        ObjectChangedStartWorkflowService _objectChangedStartWorkflowService;
        ScheduledWorkflowStartService _scheduledWorkflowStartService;
        ObjectChangedWorkflowStartService _objectChangedWorkflowStartService;

        public XpandWorkflowServer(string baseUri)
            : base(baseUri) {
            CreateServices();
        }

        void CreateServices() {
            _startWorkflowOnObjectChangeService =
                new StartWorkflowOnObjectChangeService(TimeSpan.FromSeconds(15));
            _objectChangedStartWorkflowService = new ObjectChangedStartWorkflowService();
            _scheduledWorkflowStartService = new ScheduledWorkflowStartService();
            _objectChangedWorkflowStartService = new ObjectChangedWorkflowStartService();
        }

        public XpandWorkflowServer(string baseUri, IObjectSpaceProvider servicesObjectSpaceProvider, IObjectSpaceProvider activitiesObjectSpaceProvider)
            : base(baseUri, servicesObjectSpaceProvider, activitiesObjectSpaceProvider) {
            CreateServices();
        }

        internal void InitializeDefaults() {
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
        public ObjectChangedWorkflowStartService ObjectChangedWorkflowStartService {
            get { return _objectChangedWorkflowStartService; }
        }

        public ScheduledWorkflowStartService ScheduledWorkflowStartService {
            get { return _scheduledWorkflowStartService; }
        }

        public StartWorkflowOnObjectChangeService StartWorkflowOnObjectChangeService {
            get { return _startWorkflowOnObjectChangeService; }
        }

        public ObjectChangedStartWorkflowService ObjectChangedStartWorkflowService {
            get { return _objectChangedStartWorkflowService; }
        }


    }
}