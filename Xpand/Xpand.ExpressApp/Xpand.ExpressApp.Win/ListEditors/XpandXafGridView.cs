using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Xpand.ExpressApp.Win.ListEditors {
    public class XpandXafGridView : XafGridView {
        public event EventHandler<ErrorTypeEventArgs> QueryErrorType;

        protected void OnQueryErrorType(ErrorTypeEventArgs e) {
            EventHandler<ErrorTypeEventArgs> handler = QueryErrorType;
            if (handler != null) handler(this, e);
        }

        readonly GridListEditor _gridListEditor;
        private static readonly object instanceCreated = new object();


        public XpandXafGridView() {
        }
        public XpandXafGridView(GridListEditor gridListEditor) {
            _gridListEditor = gridListEditor;
        }

        protected virtual void OnInstanceCreated(GridViewInstanceCreatedArgs e) {
            var handler = (EventHandler<GridViewInstanceCreatedArgs>)Events[instanceCreated];
            if (handler != null) handler(this, e);
        }

        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            var columnErrorType = base.GetColumnErrorType(rowHandle, column);
            var errorTypeEventArgs = new ErrorTypeEventArgs(columnErrorType, rowHandle, column);
            OnQueryErrorType(errorTypeEventArgs);
            return errorTypeEventArgs.ErrorType;
        }

        [Description("Provides the ability to customize cell merging behavior."), Category("Merge")]
        public event EventHandler<GridViewInstanceCreatedArgs> GridViewInstanceCreated {
            add { Events.AddHandler(instanceCreated, value); }
            remove { Events.RemoveHandler(instanceCreated, value); }
        }


        public Window Window { get; set; }

        public Frame MasterFrame { get; set; }
        public IModelListView ListView { get; set; }




        protected override BaseView CreateInstance() {
            var view = new XpandXafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            OnInstanceCreated(new GridViewInstanceCreatedArgs(view));
            return view;
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = ((XpandXafGridView)v);
            Window = xafGridView.Window;
            MasterFrame = xafGridView.MasterFrame;
            Events.AddHandler(instanceCreated, xafGridView.Events[instanceCreated]);
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