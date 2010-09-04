using System;
using DevExpress.ExpressApp.PivotChart;

namespace Xpand.ExpressApp.PivotChart.InPlaceEdit {
    public class EditorCreatedArgs:EventArgs {
        readonly AnalysisEditorBase _analysisEditorBase;

        public EditorCreatedArgs(AnalysisEditorBase analysisEditorBase) {
            _analysisEditorBase = analysisEditorBase;
        }

        public AnalysisEditorBase AnalysisEditorBase {
            get { return _analysisEditorBase; }
        }
    }
}