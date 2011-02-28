using System;
using DevExpress.Xpo;
using Xpand.ExpressApp.Attributes;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class JobSchedulerGroupTriggerLink : XpandCustomObject, IJobSchedulerGroupTriggerLink {
        public JobSchedulerGroupTriggerLink(Session session)
            : base(session) {
        }
        private JobSchedulerGroup _jobSchedulerGroup;
        [ProvidedAssociation("JobSchedulerGroup-JobSchedulerGroupLinks")]
        public JobSchedulerGroup JobSchedulerGroup {
            get {
                return _jobSchedulerGroup;
            }
            set {
                SetPropertyValue("JobSchedulerGroup", ref _jobSchedulerGroup, value);
            }
        }
        private XpandJobTrigger _trigger;
        [Association("JobSchedulerGroupTriggerLink-JobSchedulerGroups")]
        public XpandJobTrigger Trigger {
            get {
                return _trigger;
            }
            set {
                SetPropertyValue("Trigger", ref _trigger, value);
            }
        }
        IJobSchedulerGroup IJobSchedulerGroupTriggerLink.JobSchedulerGroup {
            get { return JobSchedulerGroup; }
            set { JobSchedulerGroup=value as JobSchedulerGroup; }
        }

        IJobTrigger IJobSchedulerGroupTriggerLink.Trigger {
            get { return Trigger; }
            set { Trigger=value as XpandJobTrigger; }
        }
    }
}