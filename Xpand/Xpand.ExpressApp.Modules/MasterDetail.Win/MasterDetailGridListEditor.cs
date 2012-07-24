using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.Win.ListEditors;
using GridListEditorSynchronizer = Xpand.ExpressApp.Win.ListEditors.GridListEditorSynchronizer;

namespace Xpand.ExpressApp.MasterDetail.Win {
    public interface IMasterDetailGridListEditor {
        event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        object DataSource { get; set; }
        GridControl Grid { get; }
        IMasterDetailXafGridView GridView { get; }
    }

    [ListEditor(typeof(object))]
    public class MasterDetailGridListEditor : GridListEditor, IMasterDetailGridListEditor {
        public MasterDetailGridListEditor(IModelListView model)
            : base(model) {
        }

        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;

        public new IMasterDetailXafGridView GridView {
            get { return (IMasterDetailXafGridView)base.GridView; }
        }

        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;
        private CollectionSourceBase _collectionSourceBase;


        public void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
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
                    var focusedView = (GridView)GridView;
                    XtraGridUtils.SelectRowByHandle(focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(focusedView)) {
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(IMasterDetailXafGridView view) {
            if (view is MasterDetailXafGridView && view.Window == null)
                return XtraGridUtils.GetFocusedRowObject(_collectionSourceBase, (ColumnView)view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return view.GetRow(rowHandle);
            return XtraGridUtils.GetFocusedRowObject(_collectionSourceBase, (ColumnView)view);
        }

        IMasterDetailXafGridView GetFocusedGridView(IMasterDetailXafGridView view) {
            Frame masterFrame = view.MasterFrame;
            return masterFrame != null && masterFrame.View != null ? (IMasterDetailXafGridView)((GridListEditor)((ListView)masterFrame.View).Editor).Grid.FocusedView : view;
        }

        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
        }

        protected override XafGridView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return (XafGridView)(gridViewCreatingEventArgs.Handled ? gridViewCreatingEventArgs.GridView : new MasterDetailXafGridView(this));
        }

        public event EventHandler<CustomGetSelectedObjectsArgs> CustomGetSelectedObjects;
        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new GridListEditorSynchronizer(this, Model);
        }
        private void OnCustomGetSelectedObjects(CustomGetSelectedObjectsArgs e) {
            EventHandler<CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public override IList GetSelectedObjects() {
            if (Grid != null && GridView != null) {
                var focusedGridView = GetFocusedGridView(GridView);
                var selectedObjects = GetSelectedObjects(focusedGridView);
                var e = new CustomGetSelectedObjectsArgs(selectedObjects);
                OnCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return base.GetSelectedObjects();
        }
        IList GetSelectedObjects(IMasterDetailXafGridView focusedView) {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0)) {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(focusedView.GetRow).Where(obj => obj != null);
                return objects.ToList();
            }
            return new List<object>();
        }

        protected override void ProcessMouseClick(EventArgs e) {
            var view = ((XafGridView)Grid.FocusedView);
            if (view.FocusedRowHandle >= 0) {
                DXMouseEventArgs mouseArgs = DXMouseEventArgs.GetMouseArgs(Grid, e);
                GridHitInfo info = GridView.CalcHitInfo(mouseArgs.Location);
                if (info.InRow && (info.HitTest == GridHitTest.RowDetail)) {
                    mouseArgs.Handled = true;
                    var showViewParameter = new ShowViewParameters();
                    var masterDetailXafGridView = (IMasterDetailXafGridView)view;
                    ListViewProcessCurrentObjectController.ShowObject(view.GetRow(view.FocusedRowHandle), showViewParameter, masterDetailXafGridView.MasterFrame.Application, masterDetailXafGridView.Window, masterDetailXafGridView.Window.View);
                    masterDetailXafGridView.MasterFrame.Application.ShowViewStrategy.ShowView(showViewParameter, new ShowViewSource(null, null));
                    return;
                }
            }

            base.ProcessMouseClick(e);
        }

        public override void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            base.Setup(collectionSource, application);
            _collectionSourceBase = collectionSource;
        }
    }
    public class MasterDetailXafGridView : XafGridView, IMasterDetailXafGridView, IQueryErrorType {
        public event EventHandler<ErrorTypeEventArgs> QueryErrorType;
        readonly GridListEditor _gridListEditor;

        protected void OnQueryErrorType(ErrorTypeEventArgs e) {
            EventHandler<ErrorTypeEventArgs> handler = QueryErrorType;
            if (handler != null) handler(this, e);
        }

        public MasterDetailXafGridView() {
        }
        public MasterDetailXafGridView(GridListEditor gridListEditor) {
            _gridListEditor = gridListEditor;
        }

        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            var columnErrorType = base.GetColumnErrorType(rowHandle, column);
            var errorTypeEventArgs = new ErrorTypeEventArgs(columnErrorType, rowHandle, column);
            OnQueryErrorType(errorTypeEventArgs);
            return errorTypeEventArgs.ErrorType;
        }

        Window IMasterDetailXafGridView.Window { get; set; }
        Frame IMasterDetailXafGridView.MasterFrame { get; set; }

        protected override BaseView CreateInstance() {
            var view = new MasterDetailXafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            return view;
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = ((IMasterDetailXafGridView)v);
            ((IMasterDetailXafGridView)this).Window = xafGridView.Window;
            ((IMasterDetailXafGridView)this).MasterFrame = xafGridView.MasterFrame;
            base.Assign(v, copyEvents);
        }
        protected override void AssignColumns(ColumnView cv, bool synchronize) {
            if (_gridListEditor == null) {
                base.AssignColumns(cv, synchronize);
                return;
            }
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                Columns.Clear();
                ////var columnsListEditorModelSynchronizer = new ColumnsListEditorModelSynchronizer(_gridListEditor, _gridListEditor.Model);
                ////columnsListEditorModelSynchronizer.ApplyModel();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<XafGridColumn>();
                foreach (var column in gridColumns) {
                    var xpandXafGridColumn = new XpandXafGridColumn(column.TypeInfo, _gridListEditor);
                    xpandXafGridColumn.ApplyModel(column.Model);
                    Columns.Add(xpandXafGridColumn);
                    xpandXafGridColumn.Assign(column);
                }
            }
        }
    }

}