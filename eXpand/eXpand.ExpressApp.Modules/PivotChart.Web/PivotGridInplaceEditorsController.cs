using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Web.ASPxEditors;
using DevExpress.Web.ASPxPivotGrid;

namespace eXpand.ExpressApp.PivotChart.Web {
    public class PivotGridInplaceEditorsController : PivotGridInplaceEditorsControllerBase {
        protected override void CreateEditors(IAnalysisControl analysisControl) {
            var pivotGridControl = ((AnalysisControlWeb)analysisControl).PivotGrid;
            pivotGridControl.CellTemplate = new CellTemplate();
        }
    }

    public class CellTemplate : ITemplate {
        #region ITemplate Members
        public void InstantiateIn(Control container) {
            var c = (PivotGridCellTemplateContainer) container;
            c.Controls.Add(new ASPxSpinEdit {Text = c.Text, Width = Unit.Percentage(100)});
        }
        #endregion
    }
}