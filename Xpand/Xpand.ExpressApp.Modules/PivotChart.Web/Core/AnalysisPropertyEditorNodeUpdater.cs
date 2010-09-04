using System;
using Xpand.ExpressApp.PivotChart.Web.PropertyEditors;

namespace Xpand.ExpressApp.PivotChart.Web.Core {
    public class AnalysisPropertyEditorNodeUpdater : PivotChart.Core.AnalysisPropertyEditorNodeUpdater
    {
        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWeb);
        }
    }
}