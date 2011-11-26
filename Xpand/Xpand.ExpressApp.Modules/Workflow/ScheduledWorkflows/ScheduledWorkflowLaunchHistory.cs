using System;
using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.ExpressApp.Workflow.ScheduledWorkflows {
    public class ScheduledWorkflowLaunchHistory : XpandCustomObject {
        public ScheduledWorkflowLaunchHistory(Session session) : base(session) { }
        public DateTime LaunchedOn {
            get { return GetPropertyValue<DateTime>("LaunchedOn"); }
            set { SetPropertyValue("LaunchedOn", value); }
        }
        [Association]
        public ScheduledWorkflow Workflow {
            get { return GetPropertyValue<ScheduledWorkflow>("Workflow"); }
            set { SetPropertyValue("Workflow", value); }
        }
    }
}
