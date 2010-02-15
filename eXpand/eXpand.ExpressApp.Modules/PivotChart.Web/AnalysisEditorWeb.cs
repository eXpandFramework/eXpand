using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.XtraCharts.Native;

namespace eXpand.ExpressApp.PivotChart.Web {
    public class AnalysisEditorWeb : DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb {
        public AnalysisEditorWeb(Type objectType, DictionaryNode info) : base(objectType, info) {
        }

        public new AnalysisControlWeb Control {
            get { return (AnalysisControlWeb) base.Control; }
        }

        protected override IAnalysisControl CreateAnalysisControl() {
            var analysisControl = new AnalysisControlWeb();
            analysisControl.Load += AnalysisControlOnLoad;
            analysisControl.PreRender += AnalysisControlOnPreRender;
            return analysisControl;
        }

        void AnalysisControlOnPreRender(object sender, EventArgs eventArgs) {
            Control.ChartTypeComboBox.SelectedIndex =
                (int) SeriesViewFactory.GetViewType(Control.Chart.SeriesTemplate.View);
        }

        void AnalysisControlOnLoad(object sender, EventArgs eventArgs) {
            ReadValue();
            Control.DataBind();
        }
    }
}