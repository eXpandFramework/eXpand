namespace eXpand.ExpressApp.PivotChart.Win {
    public class AnalysisControlVisibilityController :
        AnalysisControlVisibilityControllerBase<AnalysisEditorWin, AnalysisControlWin> {
        protected override void HidePivot(AnalysisControlWin analysisControl) {
            analysisControl.ChartControl.Parent = analysisControl;
            analysisControl.TabControl.Visible = false;
        }

        protected override void HideChart(AnalysisControlWin analysisControl) {
            analysisControl.PivotGrid.Parent = analysisControl;
            analysisControl.TabControl.Visible = false;
        }
        }
}