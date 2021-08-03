using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.ReportsV2;
using Xpand.ExpressApp.Reports.Dashboard;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.ReportsV2.Win.Dashboard {
    public class DashboardInteractionReportsController: Reports.Dashboard.DashboardInteractionReportsController {
        protected override void OnBeforeCreateDocument(DashboardInteractionController interactionController, DashboardReportViewItem dashboardReportViewItem, ListViewFilteringArgs listViewFilteringArgs){
            base.OnBeforeCreateDocument(interactionController, dashboardReportViewItem,listViewFilteringArgs);
            var objectTypeInfo = Application.TypesInfo.FindTypeInfo(((IReportDataV2) dashboardReportViewItem.ReportData).DataType);
            var reportDataTypeMember = GetReportDataTypeMember(((IModelDashboardViewItemEx)dashboardReportViewItem.Model),objectTypeInfo);
            var report = dashboardReportViewItem.Report;
            var criteria = new InOperator(reportDataTypeMember, interactionController.GetKeys(listViewFilteringArgs.DataSourceListView));
            report.FilterString = new LocalizedCriteriaWrapper(objectTypeInfo.Type, criteria).CriteriaOperator.ToString();
            report.CreateDocument(false);

        }

        private string GetReportDataTypeMember(IModelDashboardViewItemEx modelDashboardViewItemEx,ITypeInfo objectTypeInfo) {
            var reportDataTypeMember = ((IModelDashboardViewFilterReport) modelDashboardViewItemEx.Filter).ReportDataTypeMember;
            if (string.IsNullOrEmpty(reportDataTypeMember)) {
                reportDataTypeMember = objectTypeInfo.KeyMember.Name;
            }
            else {
                var memberTypeInfo = objectTypeInfo.FindMember(reportDataTypeMember).MemberTypeInfo;
                if (memberTypeInfo.IsPersistent)
                    return reportDataTypeMember + "." + memberTypeInfo.KeyMember.Name;
            }
            return reportDataTypeMember;
        }

    }
}
