using System.ComponentModel;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.Persistent.BaseImpl.JobScheduler;

namespace Xpand.ExpressApp.JobScheduler.Jobs.SendEmail {
    [CreatableItem]
    public class SendEmailJobDetailDataMap : XpandJobDetailDataMap {
        public SendEmailJobDetailDataMap(Session session)
            : base(session) {
        }
        private string _emails;
        [Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string Emails {
            get {
                return _emails;
            }
            set {
                SetPropertyValue("Emails", ref _emails, value);
            }
        }
    }
}