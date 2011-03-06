using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    public class JobTriggerTriggerListenerTriggerLink : XpandCustomObject, IJobTriggerTriggerListenerTriggerLink {
        public JobTriggerTriggerListenerTriggerLink(Session session)
            : base(session) {
        }
        private XpandJobTrigger _jobTrigger;
        private TriggerListenerTrigger _triggerListenerTrigger;
        [Association("XpandJobTrigger-JobTriggerTriggerListenerTriggerLinks")]
        public XpandJobTrigger JobTrigger {
            get {
                return _jobTrigger;
            }
            set {
                SetPropertyValue("JobTrigger", ref _jobTrigger, value);
            }
        }
        [Association("TriggerListenerTrigger-JobTriggerTriggerListenerTriggerLinks")]
        public TriggerListenerTrigger TriggerListenerTrigger {
            get {
                return _triggerListenerTrigger;
            }
            set {
                SetPropertyValue("TriggerListenerTrigger", ref _triggerListenerTrigger, value);
            }
        }

        ITriggerListenerTrigger IJobTriggerTriggerListenerTriggerLink.TriggerListenerTrigger {
            get { return TriggerListenerTrigger; }
            set { TriggerListenerTrigger=value as TriggerListenerTrigger; }
        }

        IJobTrigger IJobTriggerTriggerListenerTriggerLink.JobTrigger {
            get { return JobTrigger; }
            set { JobTrigger=value as XpandJobTrigger; }
        }

    }
}