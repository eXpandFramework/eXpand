using DevExpress.DashboardCommon;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;

namespace Xpand.ExpressApp.Dashboard.Services{
    public class XpandDashboardDataProvider : DashboardDataProvider {
        protected override IObjectDataSourceCustomFillService CreateService(IDashboardData dashboardData){
            return CreateServiceCore(dashboardData);
        }

        private static IObjectDataSourceCustomFillService CreateServiceCore(IDashboardData dashboardData){
            return dashboardData != null && ((IDashboardDefinition) dashboardData).DataViewService?(IObjectDataSourceCustomFillService) new XpandDashboardViewDataSourceFillService() : new XpandDashboardCollectionDataSourceFillService();
        }

        protected override IObjectDataSourceCustomFillService CreateViewService(IDashboardData dashboardData){
            return CreateServiceCore(dashboardData);
        }
    }
}