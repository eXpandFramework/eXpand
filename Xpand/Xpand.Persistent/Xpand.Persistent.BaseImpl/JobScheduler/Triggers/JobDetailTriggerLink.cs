using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    public class JobDetailTriggerLink : XpandCustomObject, IJobDetailTriggerLink {
        public JobDetailTriggerLink(Session session)
            : base(session) {
        }
        private XpandJobDetail _jobDetail;

        private XpandJobTrigger _jobTrigger;
        [Association("JobDetailTriggerLink-Triggers")]
        public XpandJobDetail JobDetail {
            get {
                return _jobDetail;
            }
            set {
                SetPropertyValue("JobDetail", ref _jobDetail, value);
            }
        }
        [Association("JobDetailTriggerLink-JobDetails")]
        public XpandJobTrigger JobTrigger {
            get {
                return _jobTrigger;
            }
            set {
                SetPropertyValue("JobTrigger", ref _jobTrigger, value);
            }
        }
        IJobTrigger IJobDetailTriggerLink.JobTrigger {
            get { return JobTrigger; }
            set { JobTrigger = value as XpandJobTrigger; }
        }

        IJobDetail ISupportJobDetail.JobDetail {
            get { return JobDetail; }
            set { JobDetail = value as XpandJobDetail; }
        }


    }
}