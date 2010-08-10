using System;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxEditors.FilterControl;
using DevExpress.XtraCharts.Native;
using eXpand.ExpressApp.PivotChart.Web.Editors;

namespace eXpand.ExpressApp.PivotChart.Web.PropertyEditors {
    public class AnalysisEditorWeb : DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb {
        public AnalysisEditorWeb(Type objectType, IModelMemberViewItem info)
            : base(objectType, info)
        {
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
        public new IAnalysisInfo CurrentObject
        {
            get { return (IAnalysisInfo)base.CurrentObject; }
            set { base.CurrentObject = value; }
        }

        void AnalysisControlOnLoad(object sender, EventArgs eventArgs) {
            ((IPopupFilterControlOwner) Control.PivotGrid).SettingsLoadingPanel.Enabled =((IModelPropertyEditorLoadingPanel) Model).LoadingPanel;
            if (CurrentObject.DataType!= null){
//                ReadValue();
//                Control.DataBind();
            }
        }
    }
}