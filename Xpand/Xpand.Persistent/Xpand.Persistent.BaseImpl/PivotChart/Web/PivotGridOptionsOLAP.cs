using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridOptionsOLAP : XpandBaseCustomObject, IPivotGridOptionsOLAP {
        public PivotGridOptionsOLAP(Session session)
            : base(session) {
        }
    }
}