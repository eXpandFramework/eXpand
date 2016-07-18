using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base.General;
using Xpand.ExpressApp.Reports.Dashboard;

namespace Xpand.ExpressApp.Reports.Win.Dashboard {
    public class DashboardReportViewItemController:ViewController<DashboardView>{
        protected override void OnActivated(){
            base.OnActivated();
            foreach (var item in View.GetItems<DashboardReportViewItem>()){
                item.ControlCreated+=ItemOnControlCreated;
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            foreach (var item in View.GetItems<DashboardReportViewItem>()) {
                item.ControlCreated -= ItemOnControlCreated;
            }
        }

        private void ItemOnControlCreated(object sender, EventArgs eventArgs){
            var dashboardReportViewItem = ((DashboardReportViewItem) sender);
            LoadReport(dashboardReportViewItem);
            dashboardReportViewItem.Control.PrintingSystem= dashboardReportViewItem.Report.PrintingSystem;
            if (dashboardReportViewItem.Model.CreateDocumentOnLoad)
                dashboardReportViewItem.Report.CreateDocument(true);
        }

        void LoadReport(DashboardReportViewItem dashboardReportViewItem) {
            var model = dashboardReportViewItem.Model;
            var reportDataType = ReportsModule.FindReportsModule(dashboardReportViewItem.Application.Modules).ReportDataType;
            var reportData = (IReportData)dashboardReportViewItem.View.ObjectSpace.FindObject(reportDataType, CriteriaOperator.Parse("ReportName=?", model.ReportName));
            if (reportData == null)
                throw new NullReferenceException($"Report {model.ReportName} not found");
            dashboardReportViewItem.Report=reportData.LoadReport(dashboardReportViewItem.View.ObjectSpace);
            dashboardReportViewItem.ReportData=reportData;
        }


    }
}
