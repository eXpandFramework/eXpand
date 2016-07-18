using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Xpo;
using Xpand.ExpressApp.Reports.Dashboard;

namespace Xpand.ExpressApp.ReportsV2.Win.Dashboard {
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
            var reportsModuleV2 = ReportsModuleV2.FindReportsModule(Application.Modules);
            var reportDataType = reportsModuleV2.ReportDataType;
            var reportData = (IReportDataV2) View.ObjectSpace.FindObject(reportDataType, CriteriaOperator.Parse("DisplayName=?", dashboardReportViewItem.Model.ReportName));

            if (reportData == null)
                throw new NullReferenceException($"Report {dashboardReportViewItem.Model.ReportName} not found");
            var report = ReportDataProvider.ReportsStorage.LoadReport(reportData);
            reportsModuleV2.ReportsDataSourceHelper.SetupBeforePrint(report, null, CriteriaOperator.Parse(report.FilterString), true, new SortProperty[0], true);
            dashboardReportViewItem.Report = report;
            dashboardReportViewItem.ReportData = reportData;
        }


    }
}
