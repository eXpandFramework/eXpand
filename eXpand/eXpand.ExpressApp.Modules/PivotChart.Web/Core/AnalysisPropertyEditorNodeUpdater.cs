using System;
using eXpand.ExpressApp.PivotChart.Web.PropertyEditors;

namespace eXpand.ExpressApp.PivotChart.Web.Core {
    public class AnalysisPropertyEditorNodeUpdater : PivotChart.Core.AnalysisPropertyEditorNodeUpdater
    {
        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWeb);
        }
    }
}