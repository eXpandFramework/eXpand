using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win;
using Fasterflect;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.Persistent.Base.General.Controllers.Dashboard {
    public interface IDashboardInteractionUser {
         
    }
    public class DashboardInteractionController : ViewController<DashboardView>, IModelExtender {
        public event EventHandler<ListViewFilteringArgs> ListViewFiltering;
        public event EventHandler<ListViewFilteredArgs> ListViewFiltered;

        void OnListViewFiltered(ListViewFilteredArgs e) {
            EventHandler<ListViewFilteredArgs> handler = ListViewFiltered;
            handler?.Invoke(this, e);
        }

        void OnListViewFiltering(ListViewFilteringArgs e) {
            EventHandler<ListViewFilteringArgs> handler = ListViewFiltering;
            handler?.Invoke(this, e);
        }

        readonly Dictionary<IModelListView, MasterDetailMode> _masterDetailModes = new Dictionary<IModelListView, MasterDetailMode>();

        protected override void OnDeactivated() {
            base.OnDeactivated();
            if (Frame is WinWindow)
                ((Form)Frame.Template).Shown -= Template_Shown;
            ((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance -= LayoutManagerOnCustomizeAppearance;
        }

        protected override void OnActivated() {
            base.OnActivated();
            if (Frame is WinWindow)
                ((Form)Frame.Template).Shown+=Template_Shown;

            ((ISupportAppearanceCustomization)View.LayoutManager).CustomizeAppearance += LayoutManagerOnCustomizeAppearance;
            foreach (var item in View.GetItems<DashboardViewItem>()) {
                var modelDashboardViewItem = (item.GetModel(View));
                if (!(modelDashboardViewItem is IModelDashboardReportViewItemBase))
                    AssignMasterDetailModes((IModelDashboardViewItemEx)modelDashboardViewItem);
                
            }
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            foreach (var viewItem in View.Items.OfType<DashboardViewItem>()) {
                var frame = viewItem.Frame;
                if (frame?.View is ListView listView){
                    listView.SelectionChanged -= ListViewSelectionChangedHandler;
                    listView.SelectionChanged += ListViewSelectionChangedHandler;
                }
            }
            ResetMasterDetailModes();
        }

        private void ListViewSelectionChangedHandler(object sender, EventArgs e){
            var viewItem = View?.Items
                .OfType<DashboardViewItem>()
                .FirstOrDefault(v => v.Frame != null && v.Frame.View == sender);

            if (viewItem != null)
                OnSelectionChanged(new SelectionChangedArgs((ListView) sender, viewItem));
        }

        void AssignMasterDetailModes(IModelDashboardViewItemEx modelDashboardViewItem) {
            if (modelDashboardViewItem.MasterDetailMode.HasValue) {
                if (modelDashboardViewItem.View is IModelListView modelListView) {
                    _masterDetailModes.Add(modelListView, modelListView.MasterDetailMode);
                    modelListView.MasterDetailMode = modelDashboardViewItem.MasterDetailMode.Value;
                }
            }
        }

        private void Template_Shown(object sender, EventArgs e){
            foreach (var item in View.GetItems<DashboardViewItem>().Where(item => item.Frame!=null)){
                bool focused = false;
                item.Frame.GetController<FocusDefaultDetailViewItemController>(controller => {
                    var defaultItem = controller.GetFieldValue("defaultItem");
                    if (defaultItem != null) {
                        controller.CallMethod("FocusDefaultItemControl");
                        focused = true;
                    }
                });
                if (focused)
                    break;

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
                            if (filterListView != null) OnListViewFiltered(new ListViewFilteredArgs(filterListView));
                        }
                    }
                }
            }
            NotifyControllers(selectionChangedArgs.ListView);
        }

        void NotifyControllers(ListView listView) {
            if (View != null) {
                IEnumerable<IDataSourceSelectionChanged> selectionChangeds = View.Items.OfType<DashboardViewItem>().SelectMany(Controllers);
                foreach (var selectionChanged in selectionChangeds) {
                    selectionChanged.SelectedObjects = listView.SelectedObjects;
                }
            }
        }

        IEnumerable<IDataSourceSelectionChanged> Controllers(IFrameContainer item) {
            return item.Frame?.Controllers.Cast<Controller>().Where(controller => controller.Active).OfType<IDataSourceSelectionChanged>() ?? Enumerable.Empty<IDataSourceSelectionChanged>();
        }

        ListView DataSourceListView(IModelListView dataSourceView) {
            return View != null ? (dataSourceView != null ?
                       View.Items.OfType<DashboardViewItem>().Where(item => ViewMatch(item, dataSourceView)).Select(item => item.Frame.View).Cast<ListView>().FirstOrDefault() : null)
                       : null;
        }

        bool ViewMatch(DashboardViewItem item, IModelListView dataSourceView) {
            return item.Frame?.View != null && item.Frame.View.Model == dataSourceView;
        }

        void LayoutManagerOnCustomizeAppearance(object sender, CustomizeAppearanceEventArgs customizeAppearanceEventArgs) {
            var modelDashboardViewItem = View.Model.Items.OfType<IModelDashboardViewItemEx>().FirstOrDefault(item => item.Id == customizeAppearanceEventArgs.Name);
            if (modelDashboardViewItem != null)
                ((IAppearanceVisibility)customizeAppearanceEventArgs.Item).Visibility = modelDashboardViewItem.Visibility;
        }

        ListView FilteredListView(ListView listView, DashboardViewItem dashboardViewItem, IModelDashboardViewItemEx modelDashboardViewItemFiltered) {
            var filteredColumn = modelDashboardViewItemFiltered.Filter.FilteredColumn;
            if (filteredColumn!=null) {
                var filteredListView = ((ListView)dashboardViewItem.Frame.View);
                var collectionSourceBase = filteredListView.CollectionSource;
                var criteriaSelectionOperator = CriteriaSelectionOperator(listView, filteredColumn);
                collectionSourceBase.SetCriteria(modelDashboardViewItemFiltered.Filter.DataSourceView.Id , criteriaSelectionOperator.ToString());
                return filteredListView;
            }

            return null;
        }

        CriteriaOperator CriteriaSelectionOperator(ListView listView, IModelColumn filteredColumn) {
            var keyName = filteredColumn.ModelMember.MemberInfo.MemberTypeInfo.KeyMember.Name;
            return listView.Editor is ISelectionCriteria selectionCriteria ? CriteriaOperator.Parse(filteredColumn.PropertyName + "." + (selectionCriteria).SelectionCriteria.ToString())
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
        public ListViewFilteredArgs(ListView filterListView) {
            FilterListView = filterListView;
        }

        public ListView FilterListView{ get; }
    }

    public class ListViewFilteringArgs : HandledEventArgs {
        public ListViewFilteringArgs(DashboardViewItem dashboardViewItem, IModelDashboardViewItemEx model, ListView dataSourceListView) {
            DashboardViewItem = dashboardViewItem;
            Model = model;
            DataSourceListView = dataSourceListView;
        }

        public DashboardViewItem DashboardViewItem{ get; }

        public ListView DataSourceListView{ get; }

        public IModelDashboardViewItemEx Model{ get; }
    }

    public class SelectionChangedArgs : EventArgs {
        public SelectionChangedArgs(ListView listView, DashboardViewItem dashboardViewItem) {
            ListView = listView;
            DashboardViewItem = dashboardViewItem;
        }

        public ListView ListView{ get; }

        public DashboardViewItem DashboardViewItem{ get; }

        public IModelDashboardViewItemEx DashboardViewItemModel => (IModelDashboardViewItemEx)DashboardViewItem.GetModel((DashboardView)DashboardViewItem.View);
    }
}

