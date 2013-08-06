using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.SystemModule.Search;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView;

namespace Xpand.ExpressApp.Win.SystemModule {
    public interface IModelMemberCellFilter : IModelNode {
        [Category(SearchFromViewController.AttributesCategory)]
        bool CellFilter { get; set; }
    }
    [ModelInterfaceImplementor(typeof(IModelMemberCellFilter), "ModelMember")]
    public interface IModelColumnCellFilter : IModelMemberCellFilter {
    }
    public class FilterByColumnController:ViewController<ListView>,IModelExtender {
        readonly SimpleAction _cellFilterAction;
        static readonly string _actionActiveContext = typeof(FilterByColumnController).Name;

        public FilterByColumnController() {
            _cellFilterAction = new SimpleAction(this, "CellFilter", PredefinedCategory.ObjectsCreation);
            _cellFilterAction.Execute+=CellFilterActionOnExecute;
            _cellFilterAction.SelectionDependencyType=SelectionDependencyType.RequireSingleObject;
            _cellFilterAction.Active[_actionActiveContext] = false;
        }

        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            if (GridView!=null) {
                GridView.FocusedColumnChanged+=GridViewOnFocusedColumnChanged;
            }
        }

        void GridViewOnFocusedColumnChanged(object sender, FocusedColumnChangedEventArgs focusedColumnChangedEventArgs) {
            var columnCellFilter = focusedColumnChangedEventArgs.FocusedColumn.Model() as IModelColumnCellFilter;
            if (columnCellFilter != null) _cellFilterAction.Active[_actionActiveContext] =columnCellFilter.CellFilter;
        }

        void CellFilterActionOnExecute(object sender, SimpleActionExecuteEventArgs simpleActionExecuteEventArgs) {
            var criteriaOperator = CriteriaOperator.Parse(((XafGridColumn) GridView.FocusedColumn).Model.PropertyName + "=?", GridView.GetFocusedValue());
            GridView.ActiveFilterCriteria = new GroupOperator(GroupOperatorType.And, criteriaOperator, GridView.ActiveFilterCriteria);
            GridView.ActiveFilterEnabled = true;
        }

        public GridView GridView {
            get {
                var columnsListEditor = View.Editor as ColumnsListEditor;
                return columnsListEditor != null ? columnsListEditor.GridView() : null;
            }
        }

        public SimpleAction CellFilterAction {
            get { return _cellFilterAction; }
        }

        public void ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
            extenders.Add<IModelMember,IModelMemberCellFilter>();
            extenders.Add<IModelColumn,IModelColumnCellFilter>();
        }
    }
}
