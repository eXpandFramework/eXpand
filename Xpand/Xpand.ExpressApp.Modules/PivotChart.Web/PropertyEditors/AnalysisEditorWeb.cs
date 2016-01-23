using System;
using System.Web.UI;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web.FilterControl;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting;
using Fasterflect;
using Xpand.ExpressApp.PivotChart.Web.Editors;

namespace Xpand.ExpressApp.PivotChart.Web.PropertyEditors {
    [PropertyEditor(typeof(IAnalysisInfo), true)]
    public class AnalysisEditorWeb : DevExpress.ExpressApp.PivotChart.Web.AnalysisEditorWeb, ITestableContainer ,IComplexViewItem {
        private IObjectSpace _objectSpace;

        public AnalysisEditorWeb(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
        }
        public new DevExpress.ExpressApp.PivotChart.Web.AnalysisControlWeb Control {
            get { return base.Control; }
        }

        ITestable[] ITestableContainer.GetTestableControls() {
            return new ITestable[0];
        }

        protected override IAnalysisControl CreateAnalysisControl() {
            var analysisControl = new AnalysisControlWeb();
            analysisControl.CallMethod("SetObjectSpace", _objectSpace);
            analysisControl.Load += AnalysisControlOnLoad;
            analysisControl.Unload+=AnalysisControlOnUnload;
            analysisControl.PreRender += AnalysisControlOnPreRender;
            return analysisControl;
        }

        private void AnalysisControlOnUnload(object sender, EventArgs eventArgs){
            OnControlInitialized(sender as Control);
        }

        void AnalysisControlOnPreRender(object sender, EventArgs eventArgs) {
            Control.ChartTypeComboBox.SelectedIndex =
                (int)SeriesViewFactory.GetViewType(Control.Chart.DataContainer.SeriesTemplate.View);
        }

        void AnalysisControlOnLoad(object sender, EventArgs eventArgs) {
            ((IPopupFilterControlOwner)Control.PivotGrid).SettingsLoadingPanel.Enabled = ((IModelPropertyEditorLoadingPanel)Model).LoadingPanel;
            var info = CurrentObject as IAnalysisInfo;
            if ((info != null && info.DataType != null)||!(CurrentObject is IAnalysisInfo)) {
                ReadValue();
                Control.DataBind();
                Printable = (IPrintable) this.CallMethod("GetPrintable");
            } 
        }


        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application){
            Setup(objectSpace, application);
            _objectSpace = objectSpace;
        }
    }
}