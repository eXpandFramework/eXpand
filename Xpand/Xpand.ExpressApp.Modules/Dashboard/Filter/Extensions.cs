using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DevExpress.DashboardCommon;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.Xpo;
using Xpand.Utils.Helpers;

namespace Xpand.ExpressApp.Dashboard.Filter{
    public static class Extensions{
        public static void SaveDashboard(this DevExpress.DashboardCommon.Dashboard dashboard,
            IDashboardDefinition template, MemoryStream memoryStream, XafApplication application){
            dashboard.SynchronizeModel(template, application);
            dashboard.SaveToXml(memoryStream);
            dashboard.ApplyModel(RuleMode.Always, template, application.Model);
        }

        public static DevExpress.DashboardCommon.Dashboard CreateDashBoard(this IDashboardDefinition template,
            RuleMode ruleMode, Func<Type, object> dashboardDatasource, XafApplication application){
            var dashBoard = CreateDashBoard(template, dashboardDatasource, application);
            dashBoard?.ApplyModel(ruleMode, template, application.Model);
            return dashBoard;
        }

        public static DevExpress.DashboardCommon.Dashboard CreateDashBoard(this IDashboardDefinition template,
            Func<Type, object> dashboardDatasource, XafApplication xafApplication){
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
            try{
                if (!string.IsNullOrEmpty(template.Xml)){
                    dashboard = LoadFromXml(template);
                }

                foreach (var typeWrapper in template.DashboardTypes.Select(wrapper
                    => new{
                        wrapper.Type,
                        Caption = wrapper.GetDefaultCaption((ModelApplicationBase) xafApplication.Model)
                    })){
                    var wrapper = typeWrapper;
                    var dsource = dashboard.DataSources.FirstOrDefault(source => source.Name.Equals(wrapper.Caption));
                    ProxyCollection objects = (ProxyCollection) dashboardDatasource(typeWrapper.Type);
                    if (dsource != null){
                        dsource.Data = objects;
                    }
                    else if (!dashboard.DataSources.Contains(ds => ds.Name.Equals(wrapper.Caption))){
                        var dataSource = new DashboardObjectDataSource(typeWrapper.Caption, objects){
                            Constructor =
                                new ObjectConstructorInfo(new Parameter("type", typeof(string),
                                    typeWrapper.Type.FullName))
                        };
                        dashboard.DataSources.Add(dataSource);
                    }
                }
            }
            catch (Exception e){
                dashboard.Dispose();
                Tracing.Tracer.LogError(e);
            }

            return dashboard;
        }

        static DevExpress.DashboardCommon.Dashboard LoadFromXml(IDashboardDefinition dashboardDefinition){
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
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

            return dashboard;
        }

        public static void SynchronizeModel(this DevExpress.DashboardCommon.Dashboard dashboard,
            IDashboardDefinition template, XafApplication application){
            var dataSources = GetDataSources(dashboard, RuleMode.Always, template, application.Model);
            foreach (var dataSource in dataSources){
                if (dataSource.ModelDataSource is IModelDashboardDataSourceFilter filter)
                    dataSource.DataSource.Filter = filter.SynchronizeFilter(dataSource.DataSource.Filter);
            }
        }

        public static string GetXml(this IDashboardDefinition template, RuleMode ruleMode,
            Func<Type, object> dashboardDatasource, XafApplication xafApplication){
            var dashBoard = template.CreateDashBoard(ruleMode, dashboardDatasource, xafApplication);
            using (var memoryStream = new MemoryStream()){
                dashBoard.SaveToXml(memoryStream);
                memoryStream.Position = 0;
                var document = XDocument.Load(memoryStream);
                var dataSourceAdapters = GetDataSources(dashBoard, ruleMode, template, xafApplication.Model);
                foreach (var dataSourceAdapter in dataSourceAdapters){
                    if (document.Root != null){
                        DataSourceAdapter adapter = dataSourceAdapter;
                        var datasources = document.Root.Descendants("DataSource").Where(element =>
                            element.Attribute("Name")?.Value == adapter.DataSource.Name &&
                            !element.Descendants("Filter").Any());
                        foreach (var datasource in datasources){
                            datasource.Add(new XElement("Filter", dataSourceAdapter.DataSource.Filter));
                        }
                    }
                }

                return document.ToString();
            }
        }

        public static void ApplyModel(this DevExpress.DashboardCommon.Dashboard dashboard, RuleMode ruleMode,
            IDashboardDefinition template, IModelApplication model){
            var dataSources = GetDataSources(dashboard, ruleMode, template, model);
            foreach (var adapter in dataSources){
                if (adapter.ModelDataSource is IModelDashboardDataSourceFilter filter){
                    var criteria = filter.ApplyFilter(adapter.DataSource.Filter,template);
                    adapter.DataSource.Filter = criteria.ToString();
                    if ((adapter.DataSource.Data) is ProxyCollection proxyCollection){
                        ((XPBaseCollection) proxyCollection.OriginalCollection).Criteria = criteria;
                    }
                }
                else if (adapter.ModelDataSource is IModelDashboardDataSourceParameter parameter){
                    parameter.ApplyValue(dashboard.Parameters[parameter.ParameterName]);
                }
                else if (adapter.ModelDataSource is IModelDashboardRecordsCount modelDashboardRecordsCount){
                    if ((adapter.DataSource.Data) is ProxyCollection proxyCollection)
                        ((XPBaseCollection) proxyCollection.OriginalCollection).TopReturnedObjects = modelDashboardRecordsCount.RecordsCount;
                }
            }
        }

        static IEnumerable<DataSourceAdapter> GetDataSources(DevExpress.DashboardCommon.Dashboard dashboard,
            RuleMode ruleMode, IDashboardDefinition template, IModelApplication model){
            var modelDashboardModule =
                ((IModelDashboardModuleDataSources) ((IModelApplicationDashboardModule) model).DashboardModule);
            return modelDashboardModule.DataSources
                .Where(source => source.NodeEnabled && CanApply(source, ruleMode, template)).Select(modelDataSource => {
                    var dataSource = dashboard.DataSources.FirstOrDefault(source =>
                        Regex.IsMatch(source.Name, modelDataSource.DataSourceName));
                    return new DataSourceAdapter(dataSource, modelDataSource);
                }).Where(adapter => adapter.ModelDataSource != null && adapter.DataSource != null);
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

        public static CriteriaOperator ApplyFilter(this IModelDashboardDataSourceFilter modelDataSource, string filterString,
            IDashboardDefinition template){
            string criteria = null;
            if (!string.IsNullOrEmpty(modelDataSource.Filter)){
                var criteriaOperator = CriteriaOperator.Parse(modelDataSource.Filter);
                var customFunctionValueProcessor = new CustomFunctionValueProcessor();
                customFunctionValueProcessor.Process(criteriaOperator);
                criteria = criteriaOperator.ToString();
                if (!string.IsNullOrEmpty(filterString))
                    criteria = " and " + criteria;
            }

            return CriteriaOperator.Parse(filterString + criteria);
        }
    }

    class DataSourceAdapter{
        public DataSourceAdapter(IDashboardDataSource dataSource, IModelDashboardDataSource dashboardDataSource){
            DataSource = dataSource;
            ModelDataSource = dashboardDataSource;
        }

        public IDashboardDataSource DataSource{ get; }

        public IModelDashboardDataSource ModelDataSource{ get; }
    }
}