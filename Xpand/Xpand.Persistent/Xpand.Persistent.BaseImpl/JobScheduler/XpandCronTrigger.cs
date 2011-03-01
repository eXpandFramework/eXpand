using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.JobScheduler;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("CronTrigger")]
    public class XpandCronTrigger : XpandJobTrigger, ICronTrigger {
        public XpandCronTrigger(Session session)
            : base(session) {
        }
        private string _cronExpression;
        [Size(200)]
        public string CronExpression {
            get {
                return _cronExpression;
            }
            set {
                SetPropertyValue("CronExpression", ref _cronExpression, value);
            }
        }
        private TimeZoneId _timeZone;
        public TimeZoneId TimeZone {
            get {
                return _timeZone;
            }
            set {
                SetPropertyValue("TimeZone", ref _timeZone, value);
            }
        }
    }
}