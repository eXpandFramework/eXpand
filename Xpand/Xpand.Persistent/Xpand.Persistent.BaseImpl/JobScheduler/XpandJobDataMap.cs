using DevExpress.Xpo;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.JobScheduler {
    [System.ComponentModel.DisplayName("JobDataMap")]
    public class XpandJobDataMap : XpandCustomObject {
        public XpandJobDataMap(Session session)
            : base(session) {
        }

    }
}