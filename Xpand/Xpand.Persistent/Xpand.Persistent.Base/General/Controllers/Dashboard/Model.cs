using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;

namespace Xpand.Persistent.Base.General.Controllers.Dashboard {
    public interface IModelDashboardViewFilter : IModelNode {
        [DataSourceProperty("FilteredColumns")]
        [ModelBrowsable(typeof(DashboardViewFilterVisibilityCalculator))]
        IModelColumn FilteredColumn { get; set; }

        [Browsable(false)]
        IModelList<IModelColumn> FilteredColumns { get; }
        [DataSourceProperty("DataSourceViews")]
        IModelListView DataSourceView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> DataSourceViews { get; }
    }
    public class DashboardViewFilterVisibilityCalculator : IModelIsVisible {
        #region Implementation of IModelIsVisible
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node.Parent is IModelDashboardReportViewItemBase);
        }
        #endregion
    }

    [DomainLogic(typeof(IModelDashboardViewFilter))]
    public static class DashboardViewFilteredDomainLogic {

        public static IModelList<IModelListView> Get_DataSourceViews(IModelDashboardViewFilter modelDashboardViewFilter) {
            if (modelDashboardViewFilter.FilteredColumn != null) {
                var modelClass = modelDashboardViewFilter.Application.BOModel.GetClass(modelDashboardViewFilter.FilteredColumn.ModelMember.Type);
                var dashBoardViewItems = modelDashboardViewFilter.AllDashBoardViewItems();
                var modelListViews = dashBoardViewItems.Where(item => item.View != null && item.View.AsObjectView.ModelClass == modelClass).Select(filtered => filtered.View).OfType<IModelListView>();
                return new CalculatedModelNodeList<IModelListView>(modelListViews);
            }
            if (modelDashboardViewFilter.Parent is IModelDashboardViewItemEx)
                return new CalculatedModelNodeList<IModelListView>(modelDashboardViewFilter.AllDashBoardViewItems().Select(ex => ex.View).OfType<IModelListView>());
            return new CalculatedModelNodeList<IModelListView>();
        }

        public static IEnumerable<IModelDashboardViewItemEx> AllDashBoardViewItems(this IModelDashboardViewFilter modelDashboardViewFilter) {
            return ((IModelDashboardView)modelDashboardViewFilter.Parent.Parent.Parent).Items.OfType<IModelDashboardViewItemEx>();
        }

        public static IModelList<IModelColumn> Get_FilteredColumns(IModelDashboardViewFilter modelDashboardViewFilter) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelColumn>();
            var modelListView = ((IModelDashboardViewItemEx)modelDashboardViewFilter.Parent).View as IModelListView;
            if (modelListView != null) {
                calculatedModelNodeList.AddRange(modelListView.Columns.Where(column => column.ModelMember.MemberInfo.MemberTypeInfo.IsDomainComponent));
            }
            return calculatedModelNodeList;
        }

    }
    public static class DashboardViewItemExtensions {
        public static IModelDashboardViewItem GetModel(this DashboardViewItem item, DashboardView view) {
            return (IModelDashboardViewItem)view.Model.Items[item.Id];
        }
    }

    [ModelAbstractClass]
    public interface IModelDashboardReportViewItemBase : IModelDashboardViewItem {
        string ReportName { get; set; }
        bool CreateDocumentOnLoad { get; set; }
        [Browsable(false)]
        new IModelView View { get; set; }
        [Browsable(false)]
        new string Criteria { get; set; }
        [Browsable(false)]
        [DefaultValue(ActionsToolbarVisibility.Hide)]
        new ActionsToolbarVisibility ActionsToolbarVisibility { get; set; }
        [DefaultValue(ViewItemVisibility.Show)]
        [Browsable(false)]
        ViewItemVisibility Visibility { get; set; }
        [Browsable(false)]
        MasterDetailMode? MasterDetailMode { get; set; }
    }

    [ModelAbstractClass]
    public interface IModelDashboardViewItemEx : IModelDashboardViewItem {
        [ModelBrowsable(typeof(ModelDashboardViewItemExVisibilityCalculator))]
        IModelDashboardViewFilter Filter { get; }
        [DefaultValue(ViewItemVisibility.Show)]
        [ModelBrowsable(typeof(ModelDashboardViewItemExVisibilityCalculator))]
        [Category("eXpand")]
        ViewItemVisibility Visibility { get; set; }
        [ModelBrowsable(typeof(ModelDashboardViewItemExVisibilityCalculator))]
        [Category("eXpand")]
        MasterDetailMode? MasterDetailMode { get; set; }
    }

    public class ModelDashboardViewItemExVisibilityCalculator:IModelIsVisible{
        public bool IsVisible(IModelNode node, string propertyName){
            var any = ((IModelSources) node.Application).Modules.OfType<IDashboardInteractionUser>().Any();
            if (any && propertyName == "MasterDetailMode"){
                var modelView = ((IModelDashboardViewItem) node).View;
                return modelView is IModelListView;
            }
            return any;
        }
    }
}
