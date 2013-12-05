using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Handler;
using Fasterflect;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView {
    public abstract class XpandGridView : DevExpress.XtraGrid.Views.Grid.GridView, ISupportNewItemRow {
        private ErrorMessages errorMessages;
        private BaseGridController gridController;
        private Boolean skipMakeRowVisible;
        private Boolean isNewItemRowCancelling;
        private object newItemRowObject;
        internal bool _canFilterGroupSummaryColumns = false;
        internal void SuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                SuppressInvalidCastException(column);
            }
        }

        internal void SuppressInvalidCastException(GridColumn column) {
            if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
                ((RepositoryItemLookupEdit)column.ColumnEdit).SetPropertyValue("ThrowInvalidCastException",false);
            }
        }
        internal void CancelSuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                CancelSuppressInvalidCastException(column);
            }
        }
        internal void CancelSuppressInvalidCastException(GridColumn column) {
            if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
                ((RepositoryItemLookupEdit)column.ColumnEdit).SetPropertyValue("ThrowInvalidCastException", true);
            }
        }
        protected override BaseView CreateInstance() {
            XpandGridView view = CreateInstanceView();
            view.SetGridControl(GridControl);
            return view;
        }

        protected abstract XpandGridView CreateInstanceView();

        protected override void AssignColumns(DevExpress.XtraGrid.Views.Base.ColumnView cv, bool synchronize) {
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                Columns.Clear();
                for (int n = 0; n < cv.Columns.Count; n++) {
                    if (cv.Columns[n] is IXafGridColumn) {
                        var cvColumn = (IXafGridColumn)cv.Columns[n];
                        Columns.Add((GridColumn)cvColumn.CreateNew(cvColumn.TypeInfo, cvColumn.Editor));
                        //                        Columns.Add(new XpandXafGridColumn(cvColumn.TypeInfo, cvColumn.Editor));
                    } else {
                        Columns.Add(new GridColumn());
                    }
                }
                for (int n = 0; n < Columns.Count; n++) {
                    var xpandXafGridColumn = Columns[n] as IXafGridColumn;
                    if (xpandXafGridColumn != null) {
                        (xpandXafGridColumn).Assign(cv.Columns[n]);
                    }
                }
            }
        }
        
        protected override void RaiseShownEditor() {
            if (ActiveEditor is IGridInplaceEdit) {
                ((IGridInplaceEdit)ActiveEditor).GridEditingObject = GetFocusedObject();
                if (OptionsView.NewItemRowPosition != DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None && BaseListSourceDataController.NewItemRow == FocusedRowHandle) {
                    object newObject = this.GetRow(BaseListSourceDataController.NewItemRow);
                    if (newObject != null) {
                        ((IGridInplaceEdit)ActiveEditor).GridEditingObject = newObject;
                    }
                }
            }
            base.RaiseShownEditor();
        }
        private ErrorMessage GetErrorMessage(int rowHandle, GridColumn column) {
            object listItem = GetRow(rowHandle);
            ErrorMessage errorMessage = null;
            if (column == null) {
                errorMessage = errorMessages.GetMessages(listItem);
            } else {
                errorMessage = errorMessages.GetMessage(column.FieldName, listItem);
            }
            return errorMessage;
        }
        protected override string GetColumnError(int rowHandle, GridColumn column) {
            string result = null;
            if (errorMessages != null) {
                ErrorMessage errorMessage = GetErrorMessage(rowHandle, column);
                if (errorMessage != null) {
                    result = errorMessage.Message;
                }
            } else {
                result = base.GetColumnError(rowHandle, column);
            }
            return result;
        }
        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            ErrorType result = ErrorType.Critical;
            ErrorMessage errorMessage = GetErrorMessage(rowHandle, column);
            if (errorMessage != null) {
                if (!Enum.TryParse<DevExpress.XtraEditors.DXErrorProvider.ErrorType>(errorMessage.Icon.ImageName, out result)) {
                    result = ErrorType.Critical;
                }
            }
            return result;
        }
        protected void RaiseFilterEditorPopup() {
            if (FilterEditorPopup != null) {
                FilterEditorPopup(this, EventArgs.Empty);
            }
        }
        protected void RaiseFilterEditorClosed() {
            if (FilterEditorClosed != null) {
                FilterEditorClosed(this, EventArgs.Empty);
            }
        }
        protected override bool CanBeUsedInGroupSummary(GridColumn column) {
            if (_canFilterGroupSummaryColumns) {
                XafGridColumn xafGridColumn = column as XafGridColumn;
                if (xafGridColumn != null && !xafGridColumn.AllowSummaryChange) {
                    return false;
                }
            }
            return base.CanBeUsedInGroupSummary(column);
        }
        protected override void ShowFilterPopup(GridColumn column, Rectangle bounds, Control ownerControl, object creator) {
            RaiseFilterEditorPopup();
            base.ShowFilterPopup(column, bounds, ownerControl, creator);
        }
        protected override void OnFilterPopupCloseUp(GridColumn column) {
            base.OnFilterPopupCloseUp(column);
            RaiseFilterEditorClosed();
        }
        protected override ColumnFilterInfo DoCustomFilter(GridColumn column, ColumnFilterInfo filterInfo) {
            RaiseFilterEditorPopup();
            SuppressInvalidCastException(column);
            ColumnFilterInfo result = base.DoCustomFilter(column, filterInfo);
            CancelSuppressInvalidCastException(column);
            RaiseFilterEditorClosed();
            return result;
        }
        protected override void RaiseInvalidRowException(InvalidRowExceptionEventArgs ex) {
            if (String.IsNullOrEmpty(ex.ErrorText)) {
                ex.ExceptionMode = ExceptionMode.NoAction;
            } else {
                ex.ExceptionMode = ExceptionMode.ThrowException;
            }
            base.RaiseInvalidRowException(ex);
        }
        protected override void OnActiveEditor_MouseDown(object sender, MouseEventArgs e) {
            if (ActiveEditor != null) {
                base.OnActiveEditor_MouseDown(sender, e);
            }
        }
        protected override BaseGridController CreateDataController() {
            if (requireDataControllerType == DataControllerType.AsyncServerMode) {
                gridController = new AsyncServerModeDataController();
            } else {
                if (requireDataControllerType == DataControllerType.ServerMode) {
                    gridController = new ServerModeDataController();
                } else {
                    if (DisableCurrencyManager) {
                        gridController = new GridDataController();
                    } else {
                        gridController = new XafCurrencyDataController();
                    }
                }
            }
            return gridController;
        }
        protected override RepositoryItem GetFilterRowRepositoryItem(GridColumn column, RepositoryItem current) {
            if (column.FilterMode == ColumnFilterMode.Value && current is ILookupEditRepositoryItem) {
                return current;
            }
            return base.GetFilterRowRepositoryItem(column, current);
        }
        private object GetFocusedObject() {
            return XtraGridUtils.GetFocusedRowObject(this);
        }
        protected internal void CancelCurrentRowEdit() {
            if ((gridController != null) && !gridController.IsDisposed &&
            (ActiveEditor != null) && (gridController.IsCurrentRowEditing || gridController.IsCurrentRowModified)) {
                gridController.CancelCurrentRowEdit();
            }
        }
        protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
            if (!skipMakeRowVisible) {
                base.MakeRowVisibleCore(rowHandle, invalidate);
            }
        }
        protected override void AssignActiveFilterFromFilterBuilder(CriteriaOperator newCriteria) {
            CustomiseFilterFromFilterBuilderEventArgs args = new CustomiseFilterFromFilterBuilderEventArgs(newCriteria);
            if (CustomiseFilterFromFilterBuilder != null) {
                CustomiseFilterFromFilterBuilder(this, args);
            }
            base.AssignActiveFilterFromFilterBuilder(args.Criteria);
        }
        protected internal Boolean SkipMakeRowVisible {
            get { return skipMakeRowVisible; }
            set { skipMakeRowVisible = value; }
        }
        Boolean ISupportNewItemRow.IsNewItemRowCancelling {
            get { return isNewItemRowCancelling; }
        }
        protected override FilterColumnCollection CreateFilterColumnCollection() {
            CreateCustomFilterColumnCollectionEventArgs args = new CreateCustomFilterColumnCollectionEventArgs();
            if (CreateCustomFilterColumnCollection != null) {
                CreateCustomFilterColumnCollection(this, args);
            }
            if (args.FilterColumnCollection != null) {
                return args.FilterColumnCollection;
            } else {
                return base.CreateFilterColumnCollection();
            }
        }
        public override void ShowFilterEditor(GridColumn defaultColumn) {
            RaiseFilterEditorPopup();
            SuppressInvalidCastException();
            base.ShowFilterEditor(defaultColumn);
            CancelSuppressInvalidCastException();
            RaiseFilterEditorClosed();
        }
        public override void CancelUpdateCurrentRow() {
            int updatedRowHandle = FocusedRowHandle;
            isNewItemRowCancelling = (updatedRowHandle == BaseGridController.NewItemRow);
            try {
                newItemRowObject = GetFocusedObject();
                base.CancelUpdateCurrentRow();
                if (updatedRowHandle == BaseGridController.NewItemRow) {
                    int storedFocusedHandle = FocusedRowHandle;
                    FocusedRowHandle = BaseGridController.NewItemRow;
                    if (CancelNewRow != null) {
                        CancelNewRow(this, EventArgs.Empty);
                    }
                    XtraGridUtils.SelectRowByHandle(this, storedFocusedHandle);
                } else {
                    if (RestoreCurrentRow != null) {
                        RestoreCurrentRow(this, EventArgs.Empty);
                    }
                }
            } finally {
                newItemRowObject = null;
                isNewItemRowCancelling = false;
            }
        }
        object ISupportNewItemRow.NewItemRowObject {
            get {
                return newItemRowObject;
            }
        }
        public bool IsFirstColumnInFirstRowFocused {
            get {
                return (FocusedRowHandle == 0) && (FocusedColumn == GetVisibleColumn(0));
            }
        }
        public bool IsLastColumnInLastRowFocused {
            get {
                return (FocusedRowHandle == RowCount - 1) && IsLastColumnFocused;
            }
        }
        public bool IsLastColumnFocused {
            get {
                return (FocusedColumn == GetVisibleColumn(VisibleColumns.Count - 1));
            }
        }
        public ErrorMessages ErrorMessages {
            get { return errorMessages; }
            set { errorMessages = value; }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Bitmap GetColumnBitmap(GridColumn column) {
            return Painter.GetColumnDragBitmap(ViewInfo, column, Size.Empty, false, false);
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public DevExpress.XtraGrid.Dragging.DragManager GetDragManager() {
            return ((DevExpress.XtraGrid.Views.Grid.Handler.GridHandler)Handler).DragManager;
        }
        protected internal new bool FootersIgnoreColumnFormat {
            get { return base.FootersIgnoreColumnFormat; }
        }
        public event EventHandler FilterEditorPopup;
        public event EventHandler FilterEditorClosed;
        public event EventHandler CancelNewRow;
        public event EventHandler RestoreCurrentRow;
        public event EventHandler<CreateCustomFilterColumnCollectionEventArgs> CreateCustomFilterColumnCollection;
        public event EventHandler<CustomiseFilterFromFilterBuilderEventArgs> CustomiseFilterFromFilterBuilder;
        #region Obsolete 12.1
        [Obsolete("Use CreateCustomFilterTreeNodeModel instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable 0067
        public event EventHandler<CustomDisplayPropertyNameEventArgs> CustomDisplayPropertyName;
#pragma warning restore 0067
        #endregion

    }
}