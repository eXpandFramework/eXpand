using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using DevExpress.XtraScheduler;
using Xpand.Persistent.Base.JobScheduler.Triggers;

namespace Xpand.Persistent.BaseImpl.JobScheduler.Triggers {
    [DefaultClassOptions]
    [System.ComponentModel.DisplayName("CronTrigger")]
    public class XpandCronTrigger : XpandJobTrigger, ICronTrigger {
        public XpandCronTrigger(Session session)
            : base(session) {
        }
        private string _cronExpression;
        [Size(200)]
        [RuleRequiredField]
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
        private CronTriggerMisfireInstruction _misfireInstruction;
        public CronTriggerMisfireInstruction MisfireInstruction {
            get {
                return _misfireInstruction;
            }
            set {
                SetPropertyValue("MisfireInstruction", ref _misfireInstruction, value);
            }
        }
    }
}