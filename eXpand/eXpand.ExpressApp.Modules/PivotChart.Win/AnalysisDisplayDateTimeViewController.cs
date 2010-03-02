using eXpand.ExpressApp.PivotChart.InPlaceEdit;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class AnalysisDisplayDateTimeViewController : AnalysisDisplayDateTimeViewControllerBase {
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            foreach (DevExpress.ExpressApp.PivotChart.Win.AnalysisEditorWin analysisEditor in AnalysisEditors) {
                DevExpress.ExpressApp.PivotChart.Win.AnalysisEditorWin editor = analysisEditor;
                analysisEditor.Control.FieldBuilder = new PivotGridFieldBuilder(analysisEditor.Control, s => GetPivotGroupInterval(editor,s));
            }
        }
    }
}