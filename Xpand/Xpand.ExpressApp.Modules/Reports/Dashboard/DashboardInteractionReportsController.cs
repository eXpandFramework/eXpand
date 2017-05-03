using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Controllers.Dashboard;

namespace Xpand.ExpressApp.Reports.Dashboard {
    public interface IModelDashboardViewFilterReport : IModelDashboardViewFilter {
        string ReportDataTypeMember { get; set; }
    }
    public class DashboardInteractionReportsController : ViewController<DashboardView>, IModelExtender {
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<DashboardInteractionController>(controller => controller.ListViewFiltering -= OnListViewFiltering);
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<DashboardInteractionController>(controller => controller.ListViewFiltering += OnListViewFiltering);
        }

        void OnListViewFiltering(object sender, ListViewFilteringArgs listViewFilteringArgs) {
            var dashboardViewItem = listViewFilteringArgs.DashboardViewItem;
            var dashboardReportViewItem = dashboardViewItem as DashboardReportViewItem;
            if (dashboardReportViewItem != null) {
                listViewFilteringArgs.Handled = true;
                var report = (dashboardReportViewItem).Report;
                OnBeforeCreateDocument((DashboardInteractionController)sender, dashboardReportViewItem, listViewFilteringArgs);
                report.CreateDocument(false);
            }

        }

        protected virtual void OnBeforeCreateDocument(DashboardInteractionController interactionController, DashboardReportViewItem dashboardReportViewItem, ListViewFilteringArgs listViewFilteringArgs){
            
        }

        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDashboardViewFilter, IModelDashboardViewFilterReport>();
        }
        #endregion
    }
}
