using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Reports;
using Xpand.ExpressApp.Dashboard.Core.Dashboard;

namespace Xpand.ExpressApp.Reports.Win.Dashboard {
    public interface IModelDashboardViewFilterReport : IModelDashboardViewFilter {
        string ReportDataTypeMember { get; set; }
    }
    public class DashboardInteractionReportsController : ViewController<DashboardView>, IModelExtender {
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<DashboardInteractionController>().ListViewFiltering -= OnListViewFiltering;
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<DashboardInteractionController>().ListViewFiltering += OnListViewFiltering;
        }

        string PropertyName(XafReport report, string reportDataTypeMember) {
            var typeInfo = XafTypesInfo.Instance.FindTypeInfo(report.DataType);
            return string.IsNullOrEmpty(reportDataTypeMember) ? typeInfo.KeyMember.Name : typeInfo.FindMember(reportDataTypeMember).Name;
        }

        void OnListViewFiltering(object sender, ListViewFilteringArgs listViewFilteringArgs) {

            var dashboardViewItem = listViewFilteringArgs.DashboardViewItem;
            var dashboardReportViewItem = dashboardViewItem as DashboardReportViewItem;
            if (dashboardReportViewItem != null) {
                listViewFilteringArgs.Handled = true;
                var dashboardInteractionController = ((DashboardInteractionController)sender);
                var report = (dashboardReportViewItem).Report;
                var reportDataTypeMember = ((IModelDashboardViewFilterReport)listViewFilteringArgs.Model.Filter).ReportDataTypeMember;
                var propertyName = PropertyName(report, reportDataTypeMember);
                var criteria = new InOperator(propertyName, dashboardInteractionController.Getkeys(listViewFilteringArgs.DataSourceListView));
                report.SetFilteringObject(new LocalizedCriteriaWrapper(report.DataType, criteria));
                report.CreateDocument(true);
            }

        }

        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDashboardViewFilter, IModelDashboardViewFilterReport>();
        }
        #endregion
    }
}