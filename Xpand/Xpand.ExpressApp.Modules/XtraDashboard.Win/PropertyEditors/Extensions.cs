using System.Linq;
using DevExpress.DashboardWin;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.XtraDashboard.Win.PropertyEditors{
    public static class Extensions{
        public static void Show(this DashboardViewer dashboardViewer,XafApplication application,IDashboardDefinition dashboardDefinition){
            var dashboardCollectionDataSourceFillService =(IXpandDashboardDataSourceFillService)((XpandDashboardDataProvider) DashboardsModule.DataProvider)
                .AttachViewService(dashboardViewer.ServiceContainer, (IDashboardData) dashboardDefinition);
            dashboardCollectionDataSourceFillService.FillService.LoadBeforeParameters += (sender, args) =>
                args.Handled = new[]{RuleMode.Always, RuleMode.Runtime}.Contains(dashboardDefinition.EditParameters);
            dashboardDefinition.GetDashboard(application, RuleMode.Runtime,dashboardCollectionDataSourceFillService,null,null,() => {});
            
            dashboardViewer.Dashboard = dashboardDefinition.GetDashboard(application, RuleMode.Runtime,dashboardCollectionDataSourceFillService, dashboardViewer.DataSourceOptions,
                dashboard => dashboardViewer.DashboardChanged += (_, args) => dashboardViewer.ShowDashboardParametersForm());
        }

    }
}