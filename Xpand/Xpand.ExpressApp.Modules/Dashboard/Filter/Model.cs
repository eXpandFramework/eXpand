using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.Filter{
    public interface IModelApplicationDashboardModule:IModelApplication{
        IModelDashboardModule DashboardModule { get; }
    }

    public interface IModelDashboardModule:IModelNode{
        IModelDashboarDataSources DataSources { get; }
    }
    [ModelNodesGenerator(typeof(DashboarDataSourcesNodesGenerator))]
    public interface IModelDashboarDataSources : IModelNode, IModelList<IModelDashboardDataSourceFilter> {

    }

    public class DashboarDataSourcesNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    [KeyProperty("DataSourceName"), DisplayProperty("DataSourceName")]
    [ModelDisplayName("DataSourceFilter")]
    public interface IModelDashboardDataSourceFilter : IModelNodeEnabled {
        string DataSourceName { get; set; }
        [Required]
        string Filter { get; set; }
        [CriteriaOptions("DashboardDefinitionType")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof(UITypeEditor))]
        string DashboardDefinitionCriteria { get; set; }
        [Browsable(false)]
        Type DashboardDefinitionType { get;  }
        FilterEnabled Enabled { get; set; }
    }

    public enum FilterEnabled{
        Always,
        Runtime,
        DesignTime
    }

    [DomainLogic(typeof(IModelDashboardDataSourceFilter))]
    public class ModelDashboardDataSource {
        public static Type Get_DashboardDefinitionType(IModelDashboardDataSourceFilter dashboardDataSourceFilter){
            return typeof (DashboardDefinition);
        }
    }
}