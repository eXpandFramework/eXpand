using System;
using DevExpress.DashboardCommon;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Dashboards;

namespace Xpand.ExpressApp.Dashboard.Services{
    public class XpandDashboardCollectionDataSourceFillService : DashboardCollectionDataSourceFillService,IXpandDashboardDataSourceFillService{
        public virtual XpandDashboardDataSourceFillService FillService{ get; } = new XpandDashboardDataSourceFillService();

        protected override object CreateDataSource(IObjectSpace objectSpace, Type targetType, ObjectDataSourceFillParameters objectDataSourceFillParameters){
            return FillService.CreateDataSource(objectSpace, targetType, objectDataSourceFillParameters,
                tuple => base.CreateDataSource(tuple.objectSpace, tuple.targetType,
                    tuple.objectDataSourceFillParameters));
        }
    }

    public class ObjectDataSourceCreatingArgs : EventArgs{
        public ObjectDataSourceCreatingArgs(Type targetType){
            TargetType = targetType;
        }

        public Type TargetType{ get; }
    }
}