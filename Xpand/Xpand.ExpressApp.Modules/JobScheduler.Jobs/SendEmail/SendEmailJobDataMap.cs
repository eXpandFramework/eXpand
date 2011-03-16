using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    [CreatableItem]

    public class SendEmailJobDataMap:XpandJobDataMap {
        public SendEmailJobDataMap(Session session) : base(session) {
        }
        private string _subjectTemplate;
        public string SubjectTemplate {
            get {
                return _subjectTemplate;
            }
            set {
                SetPropertyValue("SubjectTemplate", ref _subjectTemplate, value);
            }
        }
        private string _emailTemplate;
        [Size(SizeAttribute.Unlimited)]
        public string EmailTemplate {
            get {
                return _emailTemplate;
            }
            set {
                SetPropertyValue("EmailTemplate", ref _emailTemplate, value);
            }
        }
    }
}