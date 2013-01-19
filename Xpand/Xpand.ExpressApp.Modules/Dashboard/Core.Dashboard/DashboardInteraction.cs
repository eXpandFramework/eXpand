using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Dashboard.Core.Dashboard {

    public class DashboardInteractionController : ViewController<DashboardView>, IModelExtender {
        public event EventHandler<ListViewFilteringArgs> ListViewFiltering;
        public event EventHandler<ListViewFilteredArgs> ListViewFiltered;

        void OnListViewFiltered(ListViewFilteredArgs e) {
            EventHandler<ListViewFilteredArgs> handler = ListViewFiltered;
            if (handler != null) handler(this, e);
        }

        void OnListViewFiltering(ListViewFilteringArgs e) {
            EventHandler<ListViewFilteringArgs> handler = ListViewFiltering;
            if (handler != null) handler(this, e);
        }

        readonly Dictionary<IModelListView, MasterDetailMode> _masterDetailModes = new Dictionary<IModelListView, MasterDetailMode>();

        protected override void OnActivated() {
            base.OnActivated();
            ((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance += LayoutManagerOnCustomizeAppearance;
            foreach (var item in View.GetItems<DashboardViewItem>()) {
                var modelDashboardViewItem = (item.GetModel(View));
                if (!(modelDashboardViewItem is IModelDashboardReportViewItem))
                    AssignMasterDetailModes((IModelDashboardViewItemEx)modelDashboardViewItem);
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var result in View.Items.OfType<DashboardViewItem>()) {
                var frame1 = result.Frame;
                if (frame1 != null && frame1.View is ListView) {
                    var listView = ((ListView)frame1.View);
                    DashboardViewItem result1 = result;
                    listView.SelectionChanged += (sender, args) => OnSelectionChanged(new SelectionChangedArgs(listView, result1));
                }
            }
            ResetMasterDetailModes();
        }

        void AssignMasterDetailModes(IModelDashboardViewItemEx modelDashboardViewItem) {
            if (modelDashboardViewItem.MasterDetailMode.HasValue) {
                var modelListView = modelDashboardViewItem.View as IModelListView;
                if (modelListView != null) {
                    _masterDetailModes.Add(modelListView, modelListView.MasterDetailMode);
                    modelListView.MasterDetailMode = modelDashboardViewItem.MasterDetailMode.Value;
                }
            }
        }

        void ResetMasterDetailModes() {
            foreach (var masterDetailMode in _masterDetailModes) {
                masterDetailMode.Key.MasterDetailMode = masterDetailMode.Value;
            }
            _masterDetailModes.Clear();
        }

        void OnSelectionChanged(SelectionChangedArgs selectionChangedArgs) {
            var dataSourceListView = DataSourceListView((IModelListView)selectionChangedArgs.DashboardViewItemModel.View);
            if (dataSourceListView != null) {
                var dashboardViewItems = View.Items.OfType<DashboardViewItem>();
                foreach (var dashboardViewItem in dashboardViewItems) {
                    var modelDashboardViewItemEx = (IModelDashboardViewItemEx)dashboardViewItem.GetModel(View);
                    if (modelDashboardViewItemEx.Filter.DataSourceView == dataSourceListView.Model) {
                        var listViewFiltering = new ListViewFilteringArgs(dashboardViewItem, modelDashboardViewItemEx, dataSourceListView);
                        OnListViewFiltering(listViewFiltering);
                        if (!listViewFiltering.Handled) {
                            var filterListView = FilteredListView(dataSourceListView, dashboardViewItem, modelDashboardViewItemEx);
                            OnListViewFiltered(new ListViewFilteredArgs(filterListView));
                        }
                    }
                }
            }
            NotifyControllers(selectionChangedArgs.ListView);
        }

        void NotifyControllers(ListView listView) {
            if (View != null) {
                var selectionChangeds = View.Items.OfType<DashboardViewItem>().SelectMany(Controllers);
                foreach (var selectionChanged in selectionChangeds) {
                    selectionChanged.SelectedObjects = listView.SelectedObjects;
                }
            }
        }

        IEnumerable<IDataSourceSelectionChanged> Controllers(IFrameContainer item) {
            return item.Frame == null ? Enumerable.Empty<IDataSourceSelectionChanged>()
                       : item.Frame.Controllers.Cast<Controller>().Where(controller => controller.Active).OfType<IDataSourceSelectionChanged>();
        }

        ListView DataSourceListView(IModelListView dataSourceView) {
            return View != null ? (dataSourceView != null ?
                       View.Items.OfType<DashboardViewItem>().Where(item => ViewMatch(item, dataSourceView)).Select(item => item.Frame.View).Cast<ListView>().Single() : null)
                       : null;
        }

        bool ViewMatch(DashboardViewItem item, IModelListView dataSourceView) {
            return item.Frame != null && item.Frame.View != null && item.Frame.View.Model == dataSourceView;
        }

        void LayoutManagerOnCustomizeAppearance(object sender, CustomizeAppearanceEventArgs customizeAppearanceEventArgs) {
            var modelDashboardViewItem = View.Model.Items.OfType<IModelDashboardViewItemEx>().FirstOrDefault(item => item.Id == customizeAppearanceEventArgs.Name);
            if (modelDashboardViewItem != null)
                ((IAppearanceVisibility)customizeAppearanceEventArgs.Item).Visibility = modelDashboardViewItem.Visibility;
        }

        ListView FilteredListView(ListView listView, DashboardViewItem dashboardViewItem, IModelDashboardViewItemEx modelDashboardViewItemFiltered) {
            var filteredColumn = modelDashboardViewItemFiltered.Filter.FilteredColumn;
            var filteredListView = ((ListView)dashboardViewItem.Frame.View);
            var collectionSourceBase = filteredListView.CollectionSource;
            collectionSourceBase.Criteria[modelDashboardViewItemFiltered.Filter.DataSourceView.Id] = CriteriaSelectionOperator(listView, filteredColumn);
            return filteredListView;
        }

        protected override void OnDeactivated() {
            base.OnDeactivated();
            ((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance -= LayoutManagerOnCustomizeAppearance;
        }

        CriteriaOperator CriteriaSelectionOperator(ListView listView, IModelColumn filteredColumn) {
            var keyName = filteredColumn.ModelMember.MemberInfo.MemberTypeInfo.KeyMember.Name;
            var selectionCriteria = listView.Editor as ISelectionCriteria;
            return selectionCriteria != null ? CriteriaOperator.Parse(filteredColumn.PropertyName + "." + (selectionCriteria).SelectionCriteria.ToString())
                       : new InOperator(filteredColumn.PropertyName + "." + keyName, Getkeys(listView));
        }

        public IEnumerable Getkeys(ListView listView) {
            return listView.SelectedObjects.OfType<object>().Select(o => ObjectSpace.GetKeyValue(o));
        }
        #region Implementation of IModelExtender
        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelDashboardViewItem, IModelDashboardViewItemEx>();
        }
        #endregion
    }

    public class ListViewFilteredArgs : EventArgs {
        readonly ListView _filterListView;

        public ListViewFilteredArgs(ListView filterListView) {
            _filterListView = filterListView;
        }

        public ListView FilterListView {
            get { return _filterListView; }
        }
    }

    public class ListViewFilteringArgs : HandledEventArgs {
        readonly DashboardViewItem _dashboardViewItem;
        readonly IModelDashboardViewItemEx _model;
        readonly ListView _dataSourceListView;

        public ListViewFilteringArgs(DashboardViewItem dashboardViewItem, IModelDashboardViewItemEx model, ListView dataSourceListView) {
            _dashboardViewItem = dashboardViewItem;
            _model = model;
            _dataSourceListView = dataSourceListView;
        }

        public DashboardViewItem DashboardViewItem {
            get { return _dashboardViewItem; }
        }

        public ListView DataSourceListView {
            get { return _dataSourceListView; }
        }

        public IModelDashboardViewItemEx Model {
            get { return _model; }
        }
    }

    public class SelectionChangedArgs : EventArgs {
        readonly ListView _listView;
        readonly DashboardViewItem _dashboardViewItem;

        public SelectionChangedArgs(ListView listView, DashboardViewItem dashboardViewItem) {
            _listView = listView;
            _dashboardViewItem = dashboardViewItem;
        }

        public ListView ListView {
            get { return _listView; }
        }

        public DashboardViewItem DashboardViewItem {
            get {
                return _dashboardViewItem;
            }
        }

        public IModelDashboardViewItemEx DashboardViewItemModel {
            get {
                return (IModelDashboardViewItemEx)_dashboardViewItem.GetModel((DashboardView)_dashboardViewItem.View);
            }
        }
    }
}

