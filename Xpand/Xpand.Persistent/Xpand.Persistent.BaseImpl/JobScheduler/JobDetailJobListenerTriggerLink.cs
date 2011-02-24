using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class JobDetailJobListenerTriggerLink : XpandCustomObject, IJobDetailJobListenerTriggerLink {
        public JobDetailJobListenerTriggerLink(Session session)
            : base(session) {
        }
        private JobListenerTrigger _jobListenerTrigger;
        [Association("JobListenerTrigger-JobDetailJobListenerTriggerLinks")]
        public JobListenerTrigger JobListenerTrigger {
            get {
                return _jobListenerTrigger;
            }
            set {
                SetPropertyValue("JobListenerTrigger", ref _jobListenerTrigger, value);
            }
        }


        private XpandJobDetail _jobDetail;

        IJobListenerTrigger IJobDetailJobListenerTriggerLink.JobListenerTrigger {
            get { return JobListenerTrigger; }
            set { JobListenerTrigger=value as JobListenerTrigger; }
        }
        [Association("JobDetailJobListenerTriggerLink-JobListeners")]
        public XpandJobDetail JobDetail {
            get {
                return _jobDetail;
            }
            set {
                SetPropertyValue("JobDetail", ref _jobDetail, value);
            }
        }

        IJobDetail ISupportJobDetail.JobDetail {
            get { return JobDetail; }
            set { JobDetail=value as XpandJobDetail; }
        }
    }
}