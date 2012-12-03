using System;
using System.Linq;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Xpand.ExpressApp.Win.ListEditors {
    public interface IQueryErrorType {
        event EventHandler<ErrorTypeEventArgs> QueryErrorType;
    }
    [Obsolete("", true)]
    public class XpandXafGridView : XafGridView, IQueryErrorType {
        public event EventHandler<ErrorTypeEventArgs> QueryErrorType;

        protected void OnQueryErrorType(ErrorTypeEventArgs e) {
            EventHandler<ErrorTypeEventArgs> handler = QueryErrorType;
            if (handler != null) handler(this, e);
        }

        readonly GridListEditor _gridListEditor;


        public XpandXafGridView() {
        }
        public XpandXafGridView(GridListEditor gridListEditor) {
            _gridListEditor = gridListEditor;
        }

        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            var columnErrorType = base.GetColumnErrorType(rowHandle, column);
            var errorTypeEventArgs = new ErrorTypeEventArgs(columnErrorType, rowHandle, column);
            OnQueryErrorType(errorTypeEventArgs);
            return errorTypeEventArgs.ErrorType;
        }

        protected override BaseView CreateInstance() {
            var view = new XpandXafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            return view;
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
}