using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.ExpressApp.Dashboard.Services;

namespace Xpand.ExpressApp.Dashboard.DatabaseUpdate {
    public class DashboardObjectDataSourceUpdater : ModuleUpdater {
        public DashboardObjectDataSourceUpdater(IObjectSpace objectSpace, Version currentDBVersion)
            : base(objectSpace, currentDBVersion) {
        }

        public override void UpdateDatabaseAfterUpdateSchema() {
            base.UpdateDatabaseAfterUpdateSchema();
            var dashboardDefinitions = ObjectSpace.GetObjects<DashboardDefinition>().ToArray().Where(definition => definition.DashboardTypes.Count>1).ToArray();
            RemoveInvalidDataSources(dashboardDefinitions);
            RemoveDuplicates(dashboardDefinitions);
        }

        private  void RemoveDuplicates(DashboardDefinition[] dashboardDefinitions){
            foreach (var dashboardDefinition in dashboardDefinitions){
                var dublicates = dashboardDefinition.DashboardTypes
                    .GroupBy(wrapper => wrapper.Type).Where(wrappers => wrappers.Count() > 1).ToArray();
                for (var index = dublicates.Length - 1; index >= 0; index--){
                    var dublicate = dublicates[index];
                    dashboardDefinition.DashboardTypes.Remove(dublicate.First());
                }
            }
        }

        private void RemoveInvalidDataSources(IEnumerable<DashboardDefinition> dashboardDefinitions){
            foreach (var definition in dashboardDefinitions){
                var dashboard = definition.ToDashboard();
                var objectDataSources = dashboard.DataSources.OfType<DashboardObjectDataSource>().ToArray();
                for (var index = objectDataSources.Length - 1; index >= 0; index--){
                    var objectDataSource = objectDataSources[index];
                    if (objectDataSource.DataSourceType == null||XafTypesInfo.Instance.FindTypeInfo(objectDataSource.DataSourceType)==null){
                        dashboard.DataSources.Remove(objectDataSource);
                    }
                }
            }
        }
    }
}