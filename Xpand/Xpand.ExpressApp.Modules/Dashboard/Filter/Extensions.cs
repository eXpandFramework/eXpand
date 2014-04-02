using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.DashboardCommon;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.Xpo;

namespace Xpand.ExpressApp.Dashboard.Filter {
    public static class Extensions {
        public static void SaveDashboard(this IObjectSpace objectSpace, DevExpress.DashboardCommon.Dashboard dashboard, IDashboardDefinition template, MemoryStream memoryStream) {
            dashboard.SynchronizeModel(objectSpace, template);
            dashboard.SaveToXml(memoryStream);
            dashboard.ApplyModel(FilterEnabled.Always, template, objectSpace);
        }

        public static DevExpress.DashboardCommon.Dashboard CreateDashBoard(this IDashboardDefinition template, IObjectSpace objectSpace, FilterEnabled filterEnabled) {
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
            try {
                if (!string.IsNullOrEmpty(template.Xml)) {
                    dashboard = LoadFromXml(template);
                    dashboard.ApplyModel(filterEnabled, template, objectSpace);
                }
                foreach (var typeWrapper in template.DashboardTypes.Select(wrapper => new { wrapper.Type, Caption = GetCaption(wrapper) })) {
                    var wrapper = typeWrapper;
                    var dsource = dashboard.DataSources.FirstOrDefault(source => source.Name.Equals(wrapper.Caption));
                    var objects = objectSpace.CreateDashboardDataSource(wrapper.Type);
                    if (dsource != null) {
                        dsource.Data = objects;
                    }
                    else if (!dashboard.DataSources.Contains(ds => ds.Name.Equals(wrapper.Caption))) {
                        dashboard.AddDataSource(typeWrapper.Caption, objects);
                    }
                }
            }
            catch (Exception e) {
                dashboard.Dispose();
                Tracing.Tracer.LogError(e);
            }
            return dashboard;
        }

        static string GetCaption(ITypeWrapper typeWrapper) {
            var modelApplicationBase = ((ModelApplicationBase)CaptionHelper.ApplicationModel);
            var currentAspect = modelApplicationBase.CurrentAspect;
            modelApplicationBase.SetCurrentAspect("");
            var caption = typeWrapper.Caption;
            modelApplicationBase.SetCurrentAspect(currentAspect);
            return caption;
        }

        static DevExpress.DashboardCommon.Dashboard LoadFromXml(IDashboardDefinition dashboardDefinition) {
            var dashboard = new DevExpress.DashboardCommon.Dashboard();
            using (var me = new MemoryStream()) {
                var sw = new StreamWriter(me);
                sw.Write(dashboardDefinition.Xml);
                sw.Flush();
                me.Seek(0, SeekOrigin.Begin);
                dashboard.LoadFromXml(me);
                sw.Close();
                me.Close();
            }
            return dashboard;
        }

        public static void SynchronizeModel(this DevExpress.DashboardCommon.Dashboard dashboard, IObjectSpace objectSpace, IDashboardDefinition template) {
            var dataSources = GetDataSources(dashboard, FilterEnabled.Always, template, objectSpace);
            foreach (var dataSource in dataSources) {
                dataSource.DataSource.Filter = dataSource.ModelDataSource.SynchronizeFilter(dataSource.DataSource.Filter);
            }
        }

        public static void ApplyModel(this DevExpress.DashboardCommon.Dashboard dashboard, FilterEnabled filterEnabled, IDashboardDefinition template, IObjectSpace objectSpace) {
            var dataSources = GetDataSources(dashboard, filterEnabled, template, objectSpace);
            foreach (var dataSource in dataSources) {
                dataSource.DataSource.Filter = dataSource.ModelDataSource.ApplyFilter(dataSource.DataSource.Filter);
            }
        }

        private static IEnumerable<DataSourceAdapter> GetDataSources(DevExpress.DashboardCommon.Dashboard dashboard, FilterEnabled filterEnabled, IDashboardDefinition template, IObjectSpace objectSpace) {
            var modelDashboardModule =
                ((IModelApplicationDashboardModule)ApplicationHelper.Instance.Application.Model).DashboardModule;
            return modelDashboardModule.DataSources.Where(source => source.NodeEnabled && CanApply(source, filterEnabled, template, objectSpace)).Select(modelDataSource => {
                var dataSource = dashboard.DataSources.FirstOrDefault(source =>
                    String.Equals(source.Name.Trim(), modelDataSource.DataSourceName.Trim(), StringComparison.CurrentCultureIgnoreCase));
                return new DataSourceAdapter(dataSource, modelDataSource);
            }).Where(adapter => adapter.ModelDataSource != null);
        }

        private static bool CanApply(IModelDashboardDataSourceFilter modelDashboardDataSource, FilterEnabled filterEnabled, IDashboardDefinition template, IObjectSpace objectSpace) {
            if (modelDashboardDataSource.NodeEnabled&&new[]{FilterEnabled.Always,filterEnabled}.Contains(modelDashboardDataSource.Enabled)) {
                var isObjectFitForCriteria = objectSpace.IsObjectFitForCriteria(template, CriteriaOperator.Parse(modelDashboardDataSource.DashboardDefinitionCriteria));
                return isObjectFitForCriteria.HasValue && isObjectFitForCriteria.Value;
            }
            return false;
        }

        public static string SynchronizeFilter(this IModelDashboardDataSourceFilter modelDataSource, string filter){
            return !string.IsNullOrEmpty(filter) ? Regex.Replace(filter, "( and )??" + modelDataSource.Filter + "( and )?", "", RegexOptions.IgnoreCase) : null;
        }

        public static string ApplyFilter(this IModelDashboardDataSourceFilter modelDataSource, string filterString) {
            string criteria = null;
            if (!string.IsNullOrEmpty(modelDataSource.Filter)) {
                var criteriaOperator = CriteriaOperator.Parse(modelDataSource.Filter);
                new CustomFunctionValueProcessor().Process(criteriaOperator);
                criteria = criteriaOperator.ToString();
                if (!string.IsNullOrEmpty(filterString))
                    criteria = " and " + criteria;
            }
            return filterString + criteria;
        }
    }
    class DataSourceAdapter {
        private readonly DataSource _dataSource;

        private readonly IModelDashboardDataSourceFilter _dashboardDataSource;

        public DataSourceAdapter(DataSource dataSource, IModelDashboardDataSourceFilter dashboardDataSource) {
            _dataSource = dataSource;
            _dashboardDataSource = dashboardDataSource;
        }

        public DataSource DataSource {
            get { return _dataSource; }
        }

        public IModelDashboardDataSourceFilter ModelDataSource {
            get { return _dashboardDataSource; }
        }
    }

}
