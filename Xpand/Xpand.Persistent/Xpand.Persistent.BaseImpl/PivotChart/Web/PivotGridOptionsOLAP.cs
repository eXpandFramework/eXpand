using System.Diagnostics.CodeAnalysis;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart.Web;

namespace Xpand.Persistent.BaseImpl.PivotChart.Web {
    [NonPersistent][SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class PivotGridOptionsOLAP : XpandBaseCustomObject, IPivotGridOptionsOLAP {
        public PivotGridOptionsOLAP(Session session)
            : base(session) {
        }
    }
}