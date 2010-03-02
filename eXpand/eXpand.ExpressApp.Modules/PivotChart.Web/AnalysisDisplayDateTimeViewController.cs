using eXpand.ExpressApp.PivotChart.InPlaceEdit;

namespace eXpand.ExpressApp.PivotChart.Web {
    public class AnalysisDisplayDateTimeViewController : AnalysisDisplayDateTimeViewControllerBase {
        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            foreach (DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb analysisEditor in AnalysisEditors) {
                DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb editor = analysisEditor;
                analysisEditor.Control.FieldBuilder = new PivotGridFieldBuilder(analysisEditor.Control, s => GetPivotGroupInterval(editor,s));
            }
        }
    }
}