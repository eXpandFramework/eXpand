using System;
using Xpand.ExpressApp.PivotChart.Win.PropertyEditors;

namespace Xpand.ExpressApp.PivotChart.Win.Core {
    public class AnalysisPropertyEditorNodeUpdater : PivotChart.Core.AnalysisPropertyEditorNodeUpdater
    {
        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWin);
        }
    }
}