using System;

namespace eXpand.ExpressApp.PivotChart.Web {
    public sealed partial class PivotChartXpandAspNetModule : PivotChartXpandModuleBase {
        public PivotChartXpandAspNetModule() {
            InitializeComponent();
        }

        protected override Type GetPropertyEditorType() {
            return typeof(AnalysisEditorWeb);
        }
    }
}