namespace eXpand.ExpressApp.PivotChart.Win.Controllers {
    public partial class AnalysisControlVisibilityController :
        AnalysisControlVisibilityControllerBase<AnalysisEditorWin, AnalysisControlWin> {
        public AnalysisControlVisibilityController() {
            InitializeComponent();
            RegisterActions(components);
        }


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