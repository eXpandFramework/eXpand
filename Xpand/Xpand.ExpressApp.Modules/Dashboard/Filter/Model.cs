using System;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Xpo;
using Xpand.ExpressApp.Dashboard.BusinessObjects;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Dashboard.Filter{
    public interface IModelDashboardModuleDataSources:IModelNode{
        IModelDashboardDataSources DataSources { get; }
    }


    [ModelNodesGenerator(typeof(DashboarDataSourcesNodesGenerator))]
    public interface IModelDashboardDataSources : IModelNode, IModelList<IModelDashboardDataSource> {

    }

    public class DashboarDataSourcesNodesGenerator : ModelNodesGeneratorBase {
        protected override void GenerateNodesCore(ModelNode node) {

        }
    }

    [ModelAbstractClass]
    public interface IModelDashboardDataSource : IModelNodeEnabled{
        [Required]
        [DefaultValue(".*")]
        [Description("Reggex match")]
        string DataSourceName { get; set; }

        RuleMode Enabled { get; set; }
    }

    
    [ModelDisplayName("Filter")]
    public interface IModelDashboardDataSourceFilter : IModelDashboardDataSource{
        [CriteriaOptions("DashboardDefinitionType")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof (UITypeEditor))]
        string DashboardDefinitionCriteria { get; set; }

        [Browsable(false)]
        Type DashboardDefinitionType { get; }

        [Required]
        [Category("Filter")]
        string Filter { get; set; }
    }

    [ModelDisplayName("RecordsCount")]
    public interface IModelDashboardRecordsCount : IModelDashboardDataSource{
        int RecordsCount { get; set; }
    }

    [ModelDisplayName("Parameter")]
    public interface IModelDashboardDataSourceParameter : IModelDashboardDataSource {
        [Required]
        [Category("Parameter")]
        string ParameterName { get; set; }
        [Required]
        [Category("Parameter")]
        string ParameterValue { get; set; }
        [Category("Parameter")]
        bool IsCustomFunction { get; set; }
    }

    public enum RuleMode{
        Always,
        Runtime,
        DesignTime
    }

    [DomainLogic(typeof(IModelDashboardDataSource))]
    public class ModelDashboardDataSourceLogic {
        public static Type Get_DashboardDefinitionType(IModelDashboardDataSource dashboardDataSource) {
            return typeof (DashboardDefinition);
        }
    }
}