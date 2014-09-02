using System;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Reports;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Preview;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Reports.Win.Dashboard {
    public interface IModelDashboardReportViewItem : IModelDashboardReportViewItemBase {
         
    }
    [ViewItem(typeof(IModelDashboardReportViewItem))]
    public class DashboardReportViewItem : DashboardViewItem, IComplexViewItem {
        readonly IModelDashboardReportViewItem _model;
        XafReport _report;
        PrintControl _printControl;
        XafApplication _application;

        public DashboardReportViewItem(IModelDashboardReportViewItem model, Type objectType)
            : base(model, objectType) {
            _model = model;
        }

        public new IModelDashboardReportViewItem Model {
            get { return _model; }
        }

        public XafReport Report {
            get { return _report; }
        }

        protected override object CreateControlCore() {
            _printControl = new PrintControl { Dock = DockStyle.Fill };
            _printControl.ParentChanged += OnControlParentChanged;
            Type reportDataType = ReportsModule.FindReportsModule(_application.Modules).ReportDataType;
            var reportData = (IReportData)View.ObjectSpace.FindObject(reportDataType, CriteriaOperator.Parse("ReportName=?", Model.ReportName));
            if (reportData == null)
                throw new NullReferenceException(string.Format("Report {0} not found", Model.ReportName));
            _report = (XafReport)reportData.LoadReport(View.ObjectSpace);
            View.ControlsCreated += ViewOnControlsCreated;
            _printControl.PrintingSystem = Report.PrintingSystem;
            return _printControl;
        }

        void ViewOnControlsCreated(object sender, EventArgs eventArgs) {
            if (Model.CreateDocumentOnLoad)
                _report.CreateDocument(true);
        }

        void OnControlParentChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            control.ParentChanged -= OnControlParentChanged;
            var form = (XtraFormTemplateBase)control.FindForm();
            if (form == null)
                control.Parent.ParentChanged += OnControlParentChanged;
            else form.RibbonTransformer.Transformed += OnTransformed;
        }

        void OnTransformed(object sender, EventArgs e) {
            RibbonControl ribbon = ((ClassicToRibbonTransformer)sender).Ribbon;
            new PrintRibbonController { PrintControl = _printControl }.Initialize(ribbon, ribbon.StatusBar);
        }
        #region Implementation of IComplexPropertyEditor
        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
            _application = application;
        }
        #endregion

    }

}