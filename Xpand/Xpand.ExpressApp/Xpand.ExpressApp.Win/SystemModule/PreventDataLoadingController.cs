using System;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace Xpand.ExpressApp.Win.SystemModule {
    public class PreventDataLoadingController : ExpressApp.SystemModule.PreventDataLoadingController {
        private GridView _gridView;

        protected override void OnActivated() {
            base.OnActivated();
            if (IsReady()) {
                Frame.GetController<FilterControlListViewController>().CustomAssignFilterControlSourceControl += OnCustomAssignFilterControlSourceControl;
            }
        }

        protected override void OnViewControlsCreated(){
            base.OnViewControlsCreated();
            if (IsReady()){
                var columnsListEditor = View.Editor as ColumnsListEditor;
                if (columnsListEditor != null){
                    _gridView = columnsListEditor.GridView();
                    if (_gridView != null) _gridView.ActiveFilter.Changed += ActiveFilterOnChanged;
                }
            }
        }

        protected override void OnDeactivated(){
            base.OnDeactivated();
            if (_gridView!=null)
                _gridView.ActiveFilter.Changed-=ActiveFilterOnChanged;
        }

        private void ActiveFilterOnChanged(object sender, EventArgs eventArgs){
            var criteriaOperator = _gridView.ActiveFilter.Criteria;
            PreventDataLoading(criteriaOperator);
        }

        void OnCustomAssignFilterControlSourceControl(object sender, EventArgs eventArgs){
            var filteredComponentBase = View.Control as IFilteredComponentBase;
            if (filteredComponentBase != null) filteredComponentBase.RowFilterChanged += OnRowFilterChanged;
        }

        void OnRowFilterChanged(object sender, EventArgs eventArgs) {
            var criteriaOperator = Frame.GetController<FilterControlListViewController>().FilterControl.FilterCriteria;
            PreventDataLoading(criteriaOperator);
        }

    }
}