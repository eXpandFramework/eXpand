using DevExpress.Xpo;
using Xpand.Persistent.Base.PivotChart;
using Xpand.Xpo;

namespace Xpand.Persistent.BaseImpl.PivotChart {
    public class PivotOptionsChartDataSource : XpandCustomObject, IPivotOptionsChartDataSource {
        public PivotOptionsChartDataSource(Session session)
            : base(session) {
        }
    }
}