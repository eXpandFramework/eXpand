using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Xpand.Persistent.Base.JobScheduler;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    public class JobSchedulerGroup : XpandCustomObject, IJobSchedulerGroup {
        public JobSchedulerGroup(Session session)
            : base(session) {
        }
        private string _name;
        [RuleRequiredField]
        [RuleUniqueValue(null, DefaultContexts.Save)]
        public string Name {
            get {
                return _name;
            }
            set {
                SetPropertyValue("Name", ref _name, value);
            }
        }
    }
}