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

namespace Xpand.ExpressApp.Dashboard.Services{
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
        [DefaultValue(RuleMode.Always)]
        RuleMode Enabled { get; set; }

        [DefaultValue(".*")]
        [Description("Reggex match")]
        string DatasourceType{ get; set; }
    }


    [ModelDisplayName("Filter")]
    public interface IModelDashboardDataSourceFilter : IModelDashboardDataSource{
        [CriteriaOptions("DashboardDefinitionType")]
        [Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafApplication.CurrentVersion, typeof (UITypeEditor))]
        string DashboardDefinitionCriteria { get; set; }
        [Browsable(false)]
        Type DashboardDefinitionType { get; }
        string Filter { get; set; }
        int TopReturnedRecords{ get; set; }
        bool ShowInPopupView{ get; set; }
        bool ShowPersistentMembersOnly{ get; set; }
        [DefaultValue(".*")]
        [Required]
        string TableName{ get; set; }
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

    [ModelDisplayName("ParameterEdit")]
    public interface IModelDashboardDataSourceParameterEdit:IModelDashboardDataSource{
        
    }

    public enum RuleMode{
        Never,
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
