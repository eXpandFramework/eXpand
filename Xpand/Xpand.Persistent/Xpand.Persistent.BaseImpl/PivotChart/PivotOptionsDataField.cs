using System.Diagnostics.CodeAnalysis;
using DevExpress.Xpo;
using Xpand.Persistent.Base;
using Xpand.Persistent.Base.PivotChart;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    [NonPersistent][SuppressMessage("Design", "XAF0023:Do not implement IObjectSpaceLink in the XPO types")]
    public class PivotOptionsDataField : XpandBaseCustomObject, IPivotOptionsDataField {
        public PivotOptionsDataField(Session session)
            : base(session) {
        }
    }
}