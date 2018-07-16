using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.DashboardCommon;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Dashboard.Services{
    public static class Extensions{
        public static DevExpress.DashboardCommon.Dashboard GetDashboard(this IDashboardDefinition dashboardDefinition,
            XafApplication application, RuleMode ruleMode, IXpandDashboardDataSourceFillService dashboardDataSourceFillService=null,
            DataSourceOptionsContainer options=null, Action<DevExpress.DashboardCommon.Dashboard> editParameters=null, Action modeParametersEdited=null){

            var dashboard = dashboardDefinition.GetDashboard();
            dashboard.ApplyModel(ruleMode, dashboardDefinition, application.Model);
            var datasourceFilterList = dashboard.CreateDashboardDatasourceFilterList(dashboardDefinition, ruleMode,application);
            var dashboardDatasourceFilter = datasourceFilterList.Filters.FirstOrDefault();
            if (modeParametersEdited!=null){
                if (dashboardDatasourceFilter != null){
                    var detailView =
                        application.CreateDetailView(dashboardDatasourceFilter.ObjectSpace, datasourceFilterList);
                    detailView.ViewEditMode=ViewEditMode.Edit;
                    application.ShowViewStrategy.ShowViewInPopupWindow(detailView,
                        () => {
                            SaveModel(datasourceFilterList,application);
                            modeParametersEdited();
                        });
                }
                return null;
            }
            ConfigureDashboard(dashboardDefinition, ruleMode, dashboardDataSourceFillService, options, editParameters, datasourceFilterList,dashboard);
            return dashboard;
        }

        private static void SaveModel(DashboardDatasourceFilterList datasourceFilterList, XafApplication application){
            var modelDashboardDataSources = ((IModelDashboardModuleDataSources) ((IModelApplicationDashboardModule) application.Model).DashboardModule).DataSources;
            foreach (var filter in datasourceFilterList.Filters){
                if (modelDashboardDataSources[filter.ID] is IModelDashboardDataSourceFilter modelDashboardDataSource){
                    modelDashboardDataSource.Filter = filter.Filter;
                    modelDashboardDataSource.TopReturnedRecords = filter.TopReturnedRecords;
                    modelDashboardDataSource.ShowPersistentMembersOnly = filter.ShowPersistentMembersOnly;
                }
            }
        }

        private static void ConfigureDashboard(IDashboardDefinition dashboardDefinition, RuleMode ruleMode,
            IXpandDashboardDataSourceFillService dashboardCollectionDataSourceFillService, DataSourceOptionsContainer options,
            Action<DevExpress.DashboardCommon.Dashboard> editParameters,
            DashboardDatasourceFilterList datasourceFilterList, DevExpress.DashboardCommon.Dashboard dashboard){
            var canEditParameters = new[]{ruleMode, RuleMode.Always}.Contains(dashboardDefinition.EditParameters);

            if (canEditParameters)
                editParameters(dashboard);
            dashboardCollectionDataSourceFillService.FillService.DatasourceCreating += (sender, args) => {
                var filter = datasourceFilterList.Filters.LastOrDefault(_ => _.DataSourceName == args.TargetType.Name);
                if (filter != null){
                    dashboardCollectionDataSourceFillService.TopReturnedRecords = filter.TopReturnedRecords;
                    dashboardCollectionDataSourceFillService.ShowPersistentMembersOnly =
                        filter.ShowPersistentMembersOnly;
                }
            };

            if (options != null) options.ObjectDataSourceLoadingBehavior = DocumentLoadingBehavior.LoadAsIs;
        }

        static DevExpress.DashboardCommon.Dashboard GetDashboard(this IDashboardDefinition dashboardDefinition){
            var dashboard = LoadFromXml(dashboardDefinition);
            MigrateDatasourceTypes(dashboard);
            AddNewDataSources(dashboardDefinition, dashboard);

            return dashboard;
        }

        private static void AddNewDataSources(IDashboardDefinition dashboardDefinition, DevExpress.DashboardCommon.Dashboard dashboard){
            foreach (var typeWrapper in dashboardDefinition.DashboardTypes){
                var exists = dashboard.DataSources.OfType<DashboardObjectDataSource>()
                    .Any(source => Equals(source.DataSource, typeWrapper.Type));
                if (!exists){
                    var dataSource = new DashboardObjectDataSource{
                        DataSourceType = typeWrapper.Type,
                        Name = typeWrapper.Type.Name,
                        ComponentName = typeWrapper.Type.Name
                    };
                    dashboard.DataSources.Add(dataSource);
                }
            }
        }

        private static void MigrateDatasourceTypes(DevExpress.DashboardCommon.Dashboard dashboard){
            var objectDataSources = dashboard.DataSources.OfType<DashboardObjectDataSource>().ToArray();
            foreach (var objectDataSource in objectDataSources){
                var dashboardItems = dashboard.Items.OfType<DataDashboardItem>()
                    .Where(item => item.DataSource == objectDataSource).ToArray();
                if (objectDataSource.Constructor != null){
                    var typeName = objectDataSource.Constructor.Parameters.First().Value;
                    var dataSource = new DashboardObjectDataSource{
                        DataSourceType = XafTypesInfo.Instance.FindTypeInfo($"{typeName}").Type,
                        Name = objectDataSource.Name,
                        ComponentName = objectDataSource.ComponentName
                    };
                    dashboard.DataSources.Remove(objectDataSource);
                    dashboard.DataSources.Add(dataSource);
                    foreach (var dataDashboardItem in dashboardItems){
                        dataDashboardItem.DataSource = dataSource;
                    }
                }
            }
        }

        public static DevExpress.DashboardCommon.Dashboard LoadFromXml(this IDashboardDefinition dashboardDefinition){
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
            if (!string.IsNullOrWhiteSpace(dashboardDefinition.Xml)){
                using (var me = new MemoryStream()){
                    var sw = new StreamWriter(me);
                    var xml = dashboardDefinition.Xml;
                    xml = Regex.Replace(xml,
                        $"({typeof(ParameterLessProxyCollection).FullName}, {typeof(ParameterLessProxyCollection).Namespace}, Version=)([^,]*)",
                        "${1}" + XpandAssemblyInfo.Version, RegexOptions.IgnoreCase);
                    sw.Write(xml);
                    sw.Flush();
                    me.Seek(0, SeekOrigin.Begin);
                    dashboard.LoadFromXml(me);
                    sw.Close();
                    me.Close();
                }
                
            }
            return dashboard;
        }

        public static void ApplyModel(this DevExpress.DashboardCommon.Dashboard dashboard, RuleMode ruleMode,
            IDashboardDefinition template, IModelApplication model){
            var dataSources = GetDataSources(dashboard, ruleMode, template, model);
            foreach (var adapter in dataSources){
                if (adapter.ModelDataSource is IModelDashboardDataSourceFilter filter){
                    var criteria = ApplyFilter(filter.Filter, adapter.DataSource.Filter);
                    adapter.DataSource.Filter = criteria?.ToString();
                    if (adapter.DataSource is DashboardSqlDataSource dashboardSqlDataSource){
                        foreach (var selectQuery in dashboardSqlDataSource.Queries.OfType<SelectQuery>().Where(query =>Regex.IsMatch(query.Name,filter.TableName) )){
                            if (filter.TopReturnedRecords > 0)
                                selectQuery.Top = filter.TopReturnedRecords;
                            selectQuery.FilterString = criteria?.ToString();
                        }
                    }
                }
                else if (adapter.ModelDataSource is IModelDashboardDataSourceParameter parameter){
                    parameter.ApplyValue(dashboard.Parameters[parameter.ParameterName]);
                }
            }
        }

        public static DashboardDatasourceFilterList CreateDashboardDatasourceFilterList(
            this DevExpress.DashboardCommon.Dashboard dashboard, IDashboardDefinition dashboardDefinition,
            RuleMode ruleMode,XafApplication application){
            var objectSpace = application.CreateObjectSpace(typeof(DashboardDatasourceFilter));
            var dashboardDatasourceFilterList = new DashboardDatasourceFilterList();
            var dataSources = dashboard.GetDataSources(ruleMode, dashboardDefinition, application.Model);
                
            foreach (var item in dataSources.OrderBy(tuple => tuple.ModelDataSource.Index)){
                if (item.ModelDataSource is IModelDashboardDataSourceFilter modelFilter&&modelFilter.ShowInPopupView){
                    var filter = objectSpace.CreateObject<DashboardDatasourceFilter>();
                    filter.ID = item.ModelDataSource.Id();
                    filter.Filter = modelFilter.Filter;
                    filter.DataSourceName = item.DataSource.Name;
                    filter.TopReturnedRecords = modelFilter.TopReturnedRecords;
                    filter.ShowPersistentMembersOnly = modelFilter.ShowPersistentMembersOnly;
                    dashboardDatasourceFilterList.Filters.Add(filter);
                }
            }

            return dashboardDatasourceFilterList;
        }

        public static IEnumerable<(IDashboardDataSource DataSource, IModelDashboardDataSource ModelDataSource)> GetDataSources(this DevExpress.DashboardCommon.Dashboard dashboard,
            RuleMode ruleMode, IDashboardDefinition template, IModelApplication model){
            var modelDashboardModule =
                ((IModelDashboardModuleDataSources) ((IModelApplicationDashboardModule) model).DashboardModule);
            return modelDashboardModule.DataSources
                .Where(source => source.NodeEnabled && CanApply(source, ruleMode, template)).Select(modelDataSource => {
                    var dataSource = dashboard.DataSources.FirstOrDefault(source =>
                        Regex.IsMatch(source.Name, modelDataSource.DataSourceName,RegexOptions.IgnoreCase)&&Regex.IsMatch(source.GetType().Name,modelDataSource.DatasourceType,RegexOptions.IgnoreCase));
                    return (DataSource:dataSource,ModelDataSource:modelDataSource);
                }).Where(tuple => tuple.DataSource != null && tuple.ModelDataSource != null);
        }

        private static bool CanApply(IModelDashboardDataSource modelDashboardDataSource, RuleMode ruleMode,
            IDashboardDefinition template){
            if (modelDashboardDataSource.NodeEnabled &&
                new[]{RuleMode.Always, ruleMode}.Contains(modelDashboardDataSource.Enabled)){
                if (modelDashboardDataSource is IModelDashboardDataSourceFilter modelDashboardDataSourceFilter){
                    var objectSpace = XPObjectSpace.FindObjectSpaceByObject(template);
                    var isObjectFitForCriteria = objectSpace.IsObjectFitForCriteria(template,
                        CriteriaOperator.Parse(modelDashboardDataSourceFilter.DashboardDefinitionCriteria));
                    return isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value;
                }

                return true;
            }

            return false;
        }


        public static string SynchronizeFilter(this IModelDashboardDataSourceFilter modelDataSource, string filter){
            return !string.IsNullOrEmpty(filter)
                ? Regex.Replace(filter, "( and )??" + modelDataSource.Filter + "( and )?", "", RegexOptions.IgnoreCase)
                : null;
        }

        public static void ApplyValue(this IModelDashboardDataSourceParameter parameter,
            DashboardParameter dashboardParameter){
            if (parameter.IsCustomFunction){
                if (dashboardParameter != null){
                    var criteriaOperator = CriteriaOperator.Parse("Field=" + parameter.ParameterValue);
                    var customFunctionValueProcessor = new CustomFunctionValueProcessor();
                    customFunctionValueProcessor.Process(criteriaOperator);
                    dashboardParameter.Value = ((OperandValue) ((BinaryOperator) criteriaOperator).RightOperand).Value;
                }
            }
            else{
                var tryToChange = parameter.ParameterValue.TryToChange(dashboardParameter.Type, out var result);
                if (tryToChange)
                    dashboardParameter.Value = result;
            }
        }

        public static CriteriaOperator ApplyFilter(string modelRuleFilter, string filterString){
            string criteria = null;
            if (!string.IsNullOrEmpty(modelRuleFilter)){
                var criteriaOperator = CriteriaOperator.Parse(modelRuleFilter);
                var customFunctionValueProcessor = new CustomFunctionValueProcessor();
                customFunctionValueProcessor.Process(criteriaOperator);
                criteria = criteriaOperator.ToString();
                if (!string.IsNullOrEmpty(filterString))
                    criteria = " and " + criteria;
            }

            return CriteriaOperator.Parse(filterString + criteria);
        }
    }

}