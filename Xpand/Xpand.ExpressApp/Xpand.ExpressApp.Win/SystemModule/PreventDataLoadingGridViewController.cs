using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class PreventDataLoadingGridViewController : ViewController<ListView>{
        private GridView _gridView;
        private FilterControlListViewController _filterControlListViewController;
        private PreventDataLoadingController _preventDataLoadingController;

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_gridView!=null){
                _filterControlListViewController.CustomAssignFilterControlSourceControl-=OnCustomAssignFilterControlSourceControl;
            }
        }

        protected override void OnActivated() {
            base.OnActivated();
            _preventDataLoadingController = Frame.GetController<PreventDataLoadingController>();
            _filterControlListViewController = Frame.GetController<FilterControlListViewController>();
            _filterControlListViewController.CustomAssignFilterControlSourceControl += OnCustomAssignFilterControlSourceControl;
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            var columnsListEditor = View.Editor as ColumnsListEditor;
            if (columnsListEditor != null){
                _gridView = columnsListEditor.GridView();
                if (_gridView != null){
                    _gridView.ColumnFilterChanged+=GridViewOnColumnFilterChanged;
                }
            }
        }

        private void GridViewOnColumnFilterChanged(object sender, EventArgs eventArgs){
            var criteriaOperator = _gridView.ActiveFilter.Criteria;
            _preventDataLoadingController.PreventDataLoading(_gridView.ActiveFilterEnabled?criteriaOperator:null);
        }

        void OnCustomAssignFilterControlSourceControl(object sender, EventArgs eventArgs){
            var filteredComponentBase = View.Control as IFilteredComponentBase;
            if (filteredComponentBase != null) filteredComponentBase.RowFilterChanged += OnRowFilterChanged;
        }

        void OnRowFilterChanged(object sender, EventArgs eventArgs) {
            var criteriaOperator = Frame.GetController<FilterControlListViewController>().FilterControl.FilterCriteria;
            _preventDataLoadingController.PreventDataLoading(criteriaOperator);
        }

    }
}