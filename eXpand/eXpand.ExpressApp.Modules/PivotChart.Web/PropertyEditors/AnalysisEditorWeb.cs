using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Web.ASPxEditors.FilterControl;
using DevExpress.XtraCharts.Native;
using eXpand.ExpressApp.Core.DictionaryHelpers;
using eXpand.ExpressApp.PivotChart.Web.Editors;

namespace eXpand.ExpressApp.PivotChart.Web {
    public class LoadingPanelController:ViewController {
        public const string LoadingPanel = "LoadingPanel";
        public override Schema GetSchema()
        {
            return new Schema(new SchemaHelper().InjectBoolAttribute(LoadingPanel, ModelElement.DetailViewPropertyEditors));
        }
    }
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
            ((IPopupFilterControlOwner)Control.PivotGrid).SettingsLoadingPanel.Enabled = Info.GetAttributeBoolValue(LoadingPanelController.LoadingPanel,true);
            ReadValue();
            Control.DataBind();
        }
    }
}