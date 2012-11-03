using Common.Win.ChartControl;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Chart.Win;

namespace XVideoRental.Module.Win.Editors {
    [ListEditor(typeof(object), false)]
    public class XChartListEditor : XpandChartListEditor {
        public XChartListEditor(IModelListView model)
            : base(model) {
        }
    }
}
