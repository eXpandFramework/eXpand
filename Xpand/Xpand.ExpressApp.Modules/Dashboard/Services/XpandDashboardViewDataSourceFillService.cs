using System;
using DevExpress.DashboardCommon;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;
using DevExpress.ExpressApp.Xpo;

namespace Xpand.ExpressApp.Dashboard.Services{
    public class XpandDashboardViewDataSourceFillService:DashboardViewDataSourceFillService, IXpandDashboardDataSourceFillService{
        public virtual XpandDashboardDataSourceFillService FillService{ get; } = new XpandDashboardDataSourceFillService();
        public int TopReturnedRecords{ get; set; }
        bool IXpandDashboardDataSourceFillService.ShowPersistentMembersOnly{ get; set; }

        protected override object CreateDataSource(IObjectSpace objectSpace, Type targetType, ObjectDataSourceFillParameters objectDataSourceFillParameters){
            return FillService.CreateDataSource(objectSpace, targetType, objectDataSourceFillParameters,
                tuple => {
                    XpoDataView dataSource = (XpoDataView) base.CreateDataSource(tuple.objectSpace, tuple.targetType,
                        tuple.objectDataSourceFillParameters);
                    dataSource.TopReturnedObjectsCount = TopReturnedRecords;
                    return dataSource;
                });
        }    
    }
}