using DevExpress.ExpressApp;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.Core.DictionaryHelpers;

namespace eXpand.ExpressApp.PivotChart.Win.Controllers {
    public class AnalysisDisplayDateTimeViewController : AnalysisViewControllerBase {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            foreach (AnalysisEditorWin analysisEditor in AnalysisEditors){
                analysisEditor.Control.FieldBuilder = new PivotGridFieldBuilder(analysisEditor);
            }
        }
        public override Schema GetSchema()
        {
            var schemaHelper = new SchemaHelper();
            DictionaryNode injectAttribute = schemaHelper.InjectAttribute("PivotGroupInterval", typeof (PivotGroupInterval), ModelElement.Member);
            return new Schema(injectAttribute);
        }
    }
}