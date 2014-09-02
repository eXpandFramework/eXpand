using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxEditors.FilterControl;
using DevExpress.XtraCharts.Native;
using Xpand.ExpressApp.PivotChart.Web.Editors;

namespace Xpand.ExpressApp.PivotChart.Web.PropertyEditors {
    [PropertyEditor(typeof(IAnalysisInfo), true)]
    public class AnalysisEditorWeb : DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb,ITestableContainer {
        public AnalysisEditorWeb(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }
        public new AnalysisControlWeb Control {
            get { return (AnalysisControlWeb)base.Control; }
        }

        ITestable[] ITestableContainer.GetTestableControls() {
            return new ITestable[0];
        }

        protected override IAnalysisControl CreateAnalysisControl() {
            var analysisControl = new AnalysisControlWeb();
            analysisControl.Load += AnalysisControlOnLoad;
            analysisControl.PreRender += AnalysisControlOnPreRender;
            return analysisControl;
        }

        void AnalysisControlOnPreRender(object sender, EventArgs eventArgs) {
            Control.ChartTypeComboBox.SelectedIndex =
                (int)SeriesViewFactory.GetViewType(Control.Chart.DataContainer.SeriesTemplate.View);
        }

        void AnalysisControlOnLoad(object sender, EventArgs eventArgs) {
            ((IPopupFilterControlOwner)Control.PivotGrid).SettingsLoadingPanel.Enabled = ((IModelPropertyEditorLoadingPanel)Model).LoadingPanel;
            if (CurrentObject is IAnalysisInfo && ((IAnalysisInfo)CurrentObject).DataType != null) {
                ReadValue();
                Control.DataBind();
            } else if (!(CurrentObject is IAnalysisInfo)) {
                ReadValue();
                Control.DataBind();
            }
        }
    }
}