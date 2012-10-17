using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView {
    public class XpandGridListEditor : GridListEditorBase, IColumnViewEditor {
        public XpandGridListEditor(IModelListView model)
            : base(model) {
        }
        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;

        bool IColumnViewEditor.OverrideViewDesignMode { get; set; }

        DevExpress.XtraGrid.Views.Base.ColumnView IColumnViewEditor.GridView {
            get { return (DevExpress.XtraGrid.Views.Base.ColumnView)GridView; }
        }

        public new IModelListViewOptionsGridView Model {
            get { return (IModelListViewOptionsGridView)base.Model; }
        }


        bool IColumnViewEditor.IsAsyncServerMode() {
            return IsAsyncServerMode();
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            var listEditorSynchronizer = new XpandGridListEditorSynchronizer(this);
            var dynamicModelSynchronizer = new GridViewLstEditorDynamicModelSynchronizer(this);
            dynamicModelSynchronizer.ModelSynchronizerList.Insert(0, listEditorSynchronizer);
            return dynamicModelSynchronizer;
        }

        //        protected override string GetPropertyName(GridColumn gridColumn) {
        //            return gridColumn is IXafGridColumn ? ((IXafGridColumn)gridColumn).PropertyName : gridColumn.FieldName;
        //        }

        //        protected override IModelColumn GetModel(GridColumn gridColumn) {
        //            return ((IXafGridColumn)gridColumn).Model;
        //        }


        protected override ColumnWrapper CreateGridColumnWrapper(IXafGridColumn column) {
            return new XpandGridColumnWrapper(column);
        }

        protected override void ApplyModel(IXafGridColumn column, IModelColumn columnInfo) {
            var xAdvBandedGridColumn = column;
            xAdvBandedGridColumn.ApplyModel(columnInfo);
        }

        protected override IXafGridColumn CreateGridColumn() {
            return new XpandXafGridColumn(ObjectTypeInfo, this);
        }

        //        protected override GridColumn GetGridColumn(ColumnWrapper columnWrapper) {
        //            return ((GridColumnColumnWrapper)columnWrapper).Column;
        //        }
        #region modelDetailViews
        private void OnCustomGetSelectedObjects(MasterDetail.CustomGetSelectedObjectsArgs e) {
            EventHandler<MasterDetail.CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;

        protected virtual void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }

        public override object FocusedObject {
            get {
                object result = null;
                if (GridView != null) {
                    var focusedGridView = GetFocusedGridView(GridView);
                    result = GetFocusedRowObject(focusedGridView);
                    var masterDetailXafGridView = GridView;
                    Window window = masterDetailXafGridView.Window;
                    if (window != null)
                        result = masterDetailXafGridView.Window.View.ObjectSpace.GetObject(result);
                }
                return result;
            }
            set {
                if (value != null && value != DBNull.Value && GridView != null && DataSource != null) {
                    var focusedView = GridView;
                    XtraGridUtils.SelectRowByHandle((DevExpress.XtraGrid.Views.Base.ColumnView)focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle((DevExpress.XtraGrid.Views.Base.ColumnView)focusedView)) {
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(IColumnView view) {
            if (view.Window == null)
                return XtraGridUtils.GetFocusedRowObject(CollectionSource, (DevExpress.XtraGrid.Views.Base.ColumnView)view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return XtraGridUtils.GetFocusedRowObject((CollectionSourceBase)view.DataSource, (DevExpress.XtraGrid.Views.Base.ColumnView)view);
            return XtraGridUtils.GetFocusedRowObject(CollectionSource, (DevExpress.XtraGrid.Views.Base.ColumnView)view);
        }

        IColumnView GetFocusedGridView(IColumnView view) {
            Frame masterFrame = view.MasterFrame;
            return masterFrame != null && masterFrame.View != null ? GetFocusedGridView(masterFrame) : view;
        }

        IColumnView GetFocusedGridView(Frame masterFrame) {
            return (IColumnView)((IColumnViewEditor)((ListView)masterFrame.View).Editor).Grid.FocusedView;
        }

        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
        }

        protected override IColumnView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return (IColumnView)(gridViewCreatingEventArgs.Handled ? gridViewCreatingEventArgs.GridView : new XpandXafGridView(this));
        }

        public event EventHandler<MasterDetail.CustomGetSelectedObjectsArgs> CustomGetSelectedObjects;
        public override IList GetSelectedObjects() {
            if (Grid != null && GridView != null) {
                var focusedGridView = GetFocusedGridView(GridView);
                var selectedObjects = GetSelectedObjects(focusedGridView);
                var e = new MasterDetail.CustomGetSelectedObjectsArgs(selectedObjects);
                OnCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return base.GetSelectedObjects();
        }
        IList GetSelectedObjects(IColumnView focusedView) {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0)) {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(focusedView.GetRow).Where(obj => obj != null);
                return objects.ToList();
            }
            return new List<object>();
        }

        #endregion

    }
    public class ErrorTypeEventArgs : EventArgs {
        readonly int _rowHandle;
        readonly GridColumn _column;

        public ErrorTypeEventArgs(ErrorType errorType, int rowHandle, GridColumn column) {
            _rowHandle = rowHandle;
            _column = column;
            ErrorType = errorType;
        }

        public int RowHandle {
            get { return _rowHandle; }
        }

        public GridColumn Column {
            get { return _column; }
        }

        public ErrorType ErrorType { get; set; }
    }

    public interface IQueryErrorType {
        event EventHandler<ErrorTypeEventArgs> QueryErrorType;
    }

    public class XpandXafGridView : XpandGridView, IColumnView, IQueryErrorType {
        readonly GridListEditorBase _gridListEditor;

        public XpandXafGridView() {
        }

        public XpandXafGridView(GridListEditorBase gridListEditor) {
            _gridListEditor = gridListEditor;
        }

        protected override XpandGridView CreateInstanceView() {
            return new XpandXafGridView(_gridListEditor);
        }

        public event EventHandler<ErrorTypeEventArgs> QueryErrorType;

        protected void OnQueryErrorType(ErrorTypeEventArgs e) {
            EventHandler<ErrorTypeEventArgs> handler = QueryErrorType;
            if (handler != null) handler(this, e);
        }

        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            var columnErrorType = base.GetColumnErrorType(rowHandle, column);
            var errorTypeEventArgs = new ErrorTypeEventArgs(columnErrorType, rowHandle, column);
            OnQueryErrorType(errorTypeEventArgs);
            return errorTypeEventArgs.ErrorType;
        }

        #region modelDetailViews
        Window IMasterDetailColumnView.Window { get; set; }
        Frame IMasterDetailColumnView.MasterFrame { get; set; }

        bool IColumnView.CanFilterGroupSummaryColumns {
            get { return _canFilterGroupSummaryColumns; }
            set { _canFilterGroupSummaryColumns = value; }
        }

        BaseGridController IColumnView.DataController {
            get { return DataController; }
        }

        bool IColumnView.SkipMakeRowVisible {
            get { return SkipMakeRowVisible; }
            set { SkipMakeRowVisible = value; }
        }

        public new void CancelCurrentRowEdit() {
            base.CancelCurrentRowEdit();
        }

        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = ((IMasterDetailColumnView)v);
            ((IMasterDetailColumnView)this).Window = xafGridView.Window;
            ((IMasterDetailColumnView)this).MasterFrame = xafGridView.MasterFrame;
            base.Assign(v, copyEvents);
        }
        #endregion

        protected override BaseView CreateInstance() {
            var view = new XpandXafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            return view;
        }

        protected override void AssignColumns(DevExpress.XtraGrid.Views.Base.ColumnView cv, bool synchronize) {
            if (_gridListEditor == null) {
                base.AssignColumns(cv, synchronize);
                return;
            }
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                Columns.Clear();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<IXafGridColumn>();
                foreach (var column in gridColumns) {

                    var xpandXafGridColumn = column.CreateNew(column.TypeInfo, _gridListEditor);
                    xpandXafGridColumn.ApplyModel(column.Model);
                    Columns.Add((GridColumn)xpandXafGridColumn);
                    xpandXafGridColumn.Assign((GridColumn)column);
                }
            }
        }
    }


}
