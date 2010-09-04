using Xpand.ExpressApp.PivotChart.AnalysisControlVisibility;
using Xpand.ExpressApp.PivotChart.Win.Editors;
using Xpand.ExpressApp.PivotChart.Win.PropertyEditors;

namespace Xpand.ExpressApp.PivotChart.Win {
    public class AnalysisControlVisibilityController :
        AnalysisControlVisibilityControllerBase<AnalysisEditorWin, AnalysisControlWin> {
        protected override void HidePivot(AnalysisControlWin analysisControl) {
            analysisControl.ChartControl.Parent = analysisControl;
            analysisControl.TabControl.Visible = false;
            analysisControl.ChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
        }

        protected override void HideChart(AnalysisControlWin analysisControl) {
            analysisControl.PivotGrid.Parent = analysisControl;
            analysisControl.TabControl.Visible = false;
        }
        }
}