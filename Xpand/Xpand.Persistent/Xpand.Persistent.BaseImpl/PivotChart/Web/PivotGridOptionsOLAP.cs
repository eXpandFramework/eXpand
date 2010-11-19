using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart.Web;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent]
    public class PivotGridOptionsOLAP : XpandCustomObject, IPivotGridOptionsOLAP {
        public PivotGridOptionsOLAP(Session session)
            : base(session) {
        }
    }
}