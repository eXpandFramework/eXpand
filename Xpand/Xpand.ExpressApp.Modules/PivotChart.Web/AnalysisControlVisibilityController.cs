using DevExpress.ExpressApp.PivotChart.Web;
using DevExpress.Web;
using Xpand.ExpressApp.PivotChart.AnalysisControlVisibility;
using AnalysisControlWeb = Xpand.ExpressApp.PivotChart.Web.Editors.AnalysisControlWeb;
using AnalysisEditorWeb = Xpand.ExpressApp.PivotChart.Web.PropertyEditors.AnalysisEditorWeb;

namespace Xpand.ExpressApp.PivotChart.Web {
    public class AnalysisControlVisibilityController :
        AnalysisControlVisibilityControllerBase<AnalysisEditorWeb, AnalysisControlWeb> {
        protected override void HidePivot(AnalysisControlWeb analysisControl) {
            Hide(analysisControl, 0);
        }

        protected override void HideChart(AnalysisControlWeb analysisControl) {
            Hide(analysisControl, 1);
        }

        void Hide(AnalysisControlWeb analysisControl, int index) {
            ASPxPageControl asPxPageControl = analysisControl.PageControl;
            asPxPageControl.ShowTabs = false;
            asPxPageControl.TabPages[index].ClientVisible=false;
        }
    }
}