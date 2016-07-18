using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Reports;
using Xpand.ExpressApp.Reports.Dashboard;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Reports.Win.Dashboard {
    public class DashboardInteractionReportsController: Reports.Dashboard.DashboardInteractionReportsController {
        protected override void OnBeforeCreateDocument(DashboardInteractionController interactionController, DashboardReportViewItem dashboardReportViewItem, ListViewFilteringArgs listViewFilteringArgs){
            base.OnBeforeCreateDocument(interactionController, dashboardReportViewItem,listViewFilteringArgs);
            var reportDataTypeMember = ((IModelDashboardViewFilterReport) ((IModelDashboardViewItemEx)dashboardReportViewItem.Model).Filter).ReportDataTypeMember;
            var report = (XafReport)dashboardReportViewItem.Report;
            var propertyName = PropertyName(report, reportDataTypeMember);
            var criteria = new InOperator(propertyName, interactionController.Getkeys(listViewFilteringArgs.DataSourceListView));
            report.SetFilteringObject(new LocalizedCriteriaWrapper(report.DataType, criteria));
        }

        string PropertyName(XafReport report, string reportDataTypeMember) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(report.DataType);
            return string.IsNullOrEmpty(reportDataTypeMember) ? typeInfo.KeyMember.Name : typeInfo.FindMember(reportDataTypeMember).Name;
        }

    }
}
