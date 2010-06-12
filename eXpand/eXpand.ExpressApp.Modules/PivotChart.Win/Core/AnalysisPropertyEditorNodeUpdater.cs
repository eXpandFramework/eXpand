using System;
using eXpand.ExpressApp.PivotChart.Win.PropertyEditors;

namespace eXpand.ExpressApp.PivotChart.Win.Core {
    public class AnalysisPropertyEditorNodeUpdater : PivotChart.Core.AnalysisPropertyEditorNodeUpdater
    {
        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWin);
        }
    }
}