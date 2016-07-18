using System;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using Xpand.Persistent.Base.General.Controllers.Dashboard;
using Xpand.Persistent.Base.General.Win;

namespace Xpand.ExpressApp.Reports.Dashboard {
    public interface IModelDashboardReportViewItem : IModelDashboardReportViewItemBase {
         
    }
    [ViewItem(typeof(IModelDashboardReportViewItem))]
    public class DashboardReportViewItem : DashboardViewItem, IComplexViewItem {
        PrintControl _printControl;
        XafApplication _application;

        public DashboardReportViewItem(IModelDashboardReportViewItem model, Type objectType)
            : base(model, objectType) {
            Model = model;
        }

        public XafApplication Application => _application;

        public new IModelDashboardReportViewItem Model { get; }
        public XtraReport Report { get; set; }
        public object ReportData { get; set; }

        public new PrintControl Control => _printControl;

        protected override object CreateControlCore() {
            _printControl = new PrintControl { Dock = DockStyle.Fill };
            _printControl.ParentChanged += OnControlParentChanged;
            return _printControl;
        }

        void OnControlParentChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            control.ParentChanged -= OnControlParentChanged;
            var form = control.FindForm() as XtraForm;
            if (form == null){
                control.Parent.ParentChanged += OnControlParentChanged;
            }
            else{
                control.Execute(InitializePrintController);
            }
        }

        private void InitializePrintController(RibbonControl ribbon) {
            new PrintRibbonController { PrintControl = _printControl }.Initialize(ribbon, ribbon.StatusBar);
        }

        #region Implementation of IComplexPropertyEditor
        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }
        #endregion

    }

}