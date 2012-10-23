using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.PivotGrid;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid;
using Xpand.ExpressApp.SystemModule.DashBoard;
using Xpand.ExpressApp.SystemModule.Dashboard;

namespace Xpand.ExpressApp.PivotGrid.Win {
    public interface IModelDashboardViewFilterPivot : IModelDashboardViewFilter {
        [DataSourceProperty("SummaryDataSourceViews")]
        [ModelBrowsable(typeof(DashboardViewFilterVisibilityCalculator))]
        IModelListView SummaryDataSourceView { get; set; }
        [Browsable(false)]
        IModelList<IModelListView> SummaryDataSourceViews { get; }
    }
    [DomainLogic(typeof(IModelDashboardViewFilterPivot))]
    public static class DashboardViewFilteredPivotDomainLogic {
        public static IModelList<IModelListView> Get_SummaryDataSourceViews(IModelDashboardViewFilterPivot modelDashboardViewFilter) {
            var calculatedModelNodeList = new CalculatedModelNodeList<IModelListView>();
            var modelView = ((IModelDashboardViewItemEx)modelDashboardViewFilter.Parent).View;
            var dashboardViewItemFiltereds = modelDashboardViewFilter.AllDashBoardViewItems().Where(filtered => filtered.View is IModelListView && modelView != filtered.View);
            var modelListViews = dashboardViewItemFiltereds.Select(filtered => filtered.View).OfType<IModelListView>().Where(view => typeof(PivotGridListEditorBase).IsAssignableFrom(view.EditorType));
            calculatedModelNodeList.AddRange(modelListViews);
            return calculatedModelNodeList;
        }
    }

    public class DashboardInteractionPivotController : ViewController<DashboardView>, IModelExtender {
        protected override void OnDeactivated() {
            base.OnDeactivated();
            Frame.GetController<DashboardInteractionController>().ListViewFiltered -= OnListViewFiltered;
        }

        protected override void OnActivated() {
            base.OnActivated();
            Frame.GetController<DashboardInteractionController>().ListViewFiltered += OnListViewFiltered;
        }

        void OnListViewFiltered(object sender, ListViewFilteredArgs listViewFilteredArgs) {
            CreateSummaryDataSource(listViewFilteredArgs.FilterListView);
        }

        void CreateSummaryDataSource(ListView filterListView) {
            var pivotGridListEditor = filterListView.Editor as PivotGridListEditor;
            if (pivotGridListEditor != null) {
                if (!filterListView.IsControlCreated) {
                    filterListView.CreateControls();
                }
                var pivotSummaryDataSource = pivotGridListEditor.PivotGridControl.CreateSummaryDataSource();
                foreach (ListView summaryDataSourceView in GetSummaryDataSourceViews(filterListView)) {
                    ((GridControl)summaryDataSourceView.Control).DataSource = pivotSummaryDataSource;
                }
            }
        }

        IEnumerable<ListView> GetSummaryDataSourceViews(ListView filterListView) {
            var dashboardViewItems = View.Items.OfType<DashboardViewItem>();
            var viewItems = dashboardViewItems.Where(item => ((IModelDashboardViewFilterPivot)((IModelDashboardViewItemEx)item.GetModel(View)).Filter).SummaryDataSourceView == filterListView.Model);
            return viewItems.Select(item => item.Frame.View).OfType<ListView>();
        }
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDashboardViewFilter, IModelDashboardViewFilterPivot>();
        }
        #endregion
    }
}
