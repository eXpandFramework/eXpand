using DevExpress.ExpressApp;
using DevExpress.XtraPivotGrid;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.PivotChart.Win.Controllers;

namespace eXpand.ExpressApp.PivotChart.Win {
    public class AnalysisDisplayDateTimeViewController : AnalysisViewControllerBase {
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            foreach (DevExpress.ExpressApp.PivotChart.Win.AnalysisEditorWin analysisEditor in AnalysisEditors){
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