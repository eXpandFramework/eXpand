using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;

namespace Xpand.ExpressApp.SystemModule.Dashboard {
    public interface IModelDashboardViewFilter : IModelNode {
        [DataSourceProperty("FilteredColumns")]
        [ModelBrowsable(typeof(DashboardViewFilterVisibilityCalculator))]
        IModelColumn FilteredColumn { get; set; }
        string ReportDataTypeMember { get; set; }
        [Browsable(false)]
        IModelList<IModelColumn> FilteredColumns { get; }
        [DataSourceProperty("DataSourceViews")]
        IModelListView DataSourceView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> DataSourceViews { get; }

        [DataSourceProperty("SummaryDataSourceViews")]
        [ModelBrowsable(typeof(DashboardViewFilterVisibilityCalculator))]
        IModelListView SummaryDataSourceView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> SummaryDataSourceViews { get; }
    }
    public class DashboardViewFilterVisibilityCalculator : IModelIsVisible {
        #region Implementation of IModelIsVisible
        public bool IsVisible(IModelNode node, string propertyName) {
            return !(node.Parent is IModelDashboardReportViewItem);
        }
        #endregion
    }

    [DomainLogic(typeof(IModelDashboardViewFilter))]
    public static class DashboardViewFilteredDomainLogic {
        public static IModelList<IModelListView> Get_SummaryDataSourceViews(IModelDashboardViewFilter modelDashboardViewFilter) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelListView>();
            var modelView = ((IModelDashboardViewItemEx)modelDashboardViewFilter.Parent).View;
            var dashboardViewItemFiltereds = modelDashboardViewFilter.AllDashBoardViewItems().Where(filtered => filtered.View is IModelListView && modelView != filtered.View);
            throw new NotImplementedException();
            //            var modelListViews = dashboardViewItemFiltereds.Select(filtered => filtered.View).OfType<IModelListView>().Where(view => typeof(PivotGridListEditorBase).IsAssignableFrom(view.EditorType));
            //            calculatedModelNodeList.AddRange(modelListViews);
            //            return calculatedModelNodeList;
        }

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
    public interface IModelDashboardReportViewItem : IModelDashboardViewItem {
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
        IModelDashboardViewFilter Filter { get; }
        [DefaultValue(ViewItemVisibility.Show)]
        ViewItemVisibility Visibility { get; set; }
        MasterDetailMode? MasterDetailMode { get; set; }
    }
}
