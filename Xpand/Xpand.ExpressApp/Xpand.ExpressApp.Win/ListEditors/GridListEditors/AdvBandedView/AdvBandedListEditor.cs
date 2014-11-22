using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Xpo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView {
    [ListEditor(typeof(object), false)]
    public class AdvBandedListEditor : XpandGridListEditor {
        public AdvBandedListEditor(IModelListView model)
            : base(model) {
        }

        public new IModelListViewOptionsAdvBandedView Model {
            get { return (IModelListViewOptionsAdvBandedView)base.Model; }
        }

        public new AdvBandedGridView GridView {
            get { return (AdvBandedGridView)base.GridView; }
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView CreateGridViewCore() {
            return new AdvBandedGridView(this);
        }

        protected override List<IModelSynchronizable> CreateModelSynchronizers() {
            return new AdvBandedViewLstEditorDynamicModelSynchronizer(this).ModelSynchronizerList;
        }

        protected override void OnCreateCustomColumn(object sender, CreateCustomColumnEventArgs e) {
            base.OnCreateCustomColumn(sender, e);
            if (e.Column == null) {
                e.Column = new BandedGridColumn();
            }
        }
    }
    public class AdvBandedGridView : DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView, IMasterDetailColumnView, IXafGridView, ISupportNewItemRow, IModelSynchronizersHolder {
        public Dictionary<Component, IModelSynchronizer> ColumnsInfoCache = new Dictionary<Component, IModelSynchronizer>();
        #region IModelSynchronizersHolder
        IModelSynchronizer IModelSynchronizersHolder.GetSynchronizer(Component component) {
            IModelSynchronizer result = null;
            if (component != null) {
                result = OnCustomModelSynchronizer(component);
                if (result == null) {
                    ColumnsInfoCache.TryGetValue(component, out result);
                }
            }
            return result;
        }
        void IModelSynchronizersHolder.RegisterSynchronizer(Component component, IModelSynchronizer modelSynchronizer) {
            ColumnsInfoCache.Add(component, modelSynchronizer);
        }
        void IModelSynchronizersHolder.RemoveSynchronizer(Component component) {
            if (component != null && ColumnsInfoCache.ContainsKey(component)) {
                ColumnsInfoCache.Remove(component);
            }
        }
        void IModelSynchronizersHolder.AssignSynchronizers(DevExpress.XtraGrid.Views.Base.ColumnView sourceView) {
            var current = (IModelSynchronizersHolder)this;
            var sourceInfoProvider = (IModelSynchronizersHolder)sourceView;
            for (int n = 0; n < sourceView.Columns.Count; n++) {
                var info = sourceInfoProvider.GetSynchronizer(sourceView.Columns[n]) as IGridColumnModelSynchronizer;
                if (info != null) {
                    current.RegisterSynchronizer(Columns[n], info);
                }
            }
        }
        private event EventHandler<CustomModelSynchronizerEventArgs> CustomModelSynchronizer;
        event EventHandler<CustomModelSynchronizerEventArgs> IModelSynchronizersHolder.CustomModelSynchronizer {
            add { CustomModelSynchronizer += value; }
            remove { CustomModelSynchronizer -= value; }
        }
        protected virtual IModelSynchronizer OnCustomModelSynchronizer(Component component) {
            if (CustomModelSynchronizer != null) {
                var args = new CustomModelSynchronizerEventArgs(component);
                CustomModelSynchronizer(this, args);
                return args.ModelSynchronizer;
            }
            return null;
        }
        #endregion
        private ErrorMessages _errorMessages;
        readonly AdvBandedListEditor _gridListEditor;
        bool _isNewItemRowCancelling;
        object _newItemRowObject;
        BaseGridController _gridController;

        public AdvBandedGridView() {
        }

        public override void CancelUpdateCurrentRow() {
            int updatedRowHandle = FocusedRowHandle;
            _isNewItemRowCancelling = (updatedRowHandle == BaseListSourceDataController.NewItemRow);
            try {
                _newItemRowObject = GetFocusedObject();
                base.CancelUpdateCurrentRow();
                if (updatedRowHandle == BaseListSourceDataController.NewItemRow) {
                    int storedFocusedHandle = FocusedRowHandle;
                    FocusedRowHandle = BaseListSourceDataController.NewItemRow;
                    if (CancelNewRow != null) {
                        CancelNewRow(this, EventArgs.Empty);
                    }
                    XtraGridUtils.SelectRowByHandle(this, storedFocusedHandle);
                }
                else {
                    if (RestoreCurrentRow != null) {
                        RestoreCurrentRow(this, EventArgs.Empty);
                    }
                }
            }
            finally {
                _newItemRowObject = null;
                _isNewItemRowCancelling = false;
            }
        }
        object ISupportNewItemRow.NewItemRowObject {
            get {
                return _newItemRowObject;
            }
        }

        private object GetFocusedObject() {
            return XtraGridUtils.GetFocusedRowObject(this);
        }

        public AdvBandedGridView(AdvBandedListEditor gridListEditor)
            : base(gridListEditor.Grid) {
            _gridListEditor = gridListEditor;
        }

        protected override void RaiseShownEditor() {
            var gridInplaceEdit = ActiveEditor as IGridInplaceEdit;
            if (gridInplaceEdit != null) {
                var focusedRowObject = XtraGridUtils.GetFocusedRowObject(this);
                if (focusedRowObject is IXPSimpleObject) {
                    (gridInplaceEdit).GridEditingObject = focusedRowObject;
                }
            }
            base.RaiseShownEditor();
        }
        protected override bool DoIncrementalSearch(string text) {
            if (!string.IsNullOrEmpty(text) && VisibleColumns.Count < 2 && OptionsSelection.EnableAppearanceFocusedRow) {
                OptionsSelection.EnableAppearanceFocusedRow = false;
            }
            return base.DoIncrementalSearch(text);
        }
        protected override BaseView CreateInstance() {
            var view = new AdvBandedGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            return view;
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = (IMasterDetailColumnView)v;
            ((IMasterDetailColumnView)this).Window = xafGridView.Window;
            ((IMasterDetailColumnView)this).MasterFrame = xafGridView.MasterFrame;
            base.Assign(v, copyEvents);
        }

        protected override string GetColumnError(int rowHandle, GridColumn column) {
            string result = null;
            if (_errorMessages != null) {
                object listItem = GetRow(rowHandle);
                ErrorMessage message = column == null ? _errorMessages.GetMessages(listItem) : _errorMessages.GetMessage(column.FieldName, listItem);
                if (message != null) result = message.Message;
            }
            else {
                result = base.GetColumnError(rowHandle, column);
            }
            return result;
        }
        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            return ErrorType.Critical;
        }
        protected override void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) {
            if (!OptionsSelection.EnableAppearanceFocusedRow && State != BandedGridState.IncrementalSearch) {
                OptionsSelection.EnableAppearanceFocusedRow = true;
            }
            base.DoChangeFocusedRow(currentRowHandle, newRowHandle, raiseUpdateCurrentRow);
        }
        protected override void RaiseInvalidRowException(InvalidRowExceptionEventArgs ex) {
            ex.ExceptionMode = ExceptionMode.ThrowException;
            base.RaiseInvalidRowException(ex);
        }
        public void ForceLoaded() {
            OnLoaded();
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
        protected void RaiseFilterEditorPopup() {
            if (FilterEditorPopup != null) {
                FilterEditorPopup(this, EventArgs.Empty);
            }
        }
        protected override void ShowFilterPopup(GridColumn column, Rectangle bounds, Control ownerControl, object creator) {
            RaiseFilterEditorPopup();
            base.ShowFilterPopup(column, bounds, ownerControl, creator);
        }
        protected void RaiseFilterEditorClosed() {
            if (FilterEditorClosed != null) {
                FilterEditorClosed(this, EventArgs.Empty);
            }
        }
        protected override void OnFilterPopupCloseUp(GridColumn column) {
            base.OnFilterPopupCloseUp(column);
            RaiseFilterEditorClosed();
        }

        protected override FilterColumnCollection CreateFilterColumnCollection() {
            var args = new CreateCustomFilterColumnCollectionEventArgs();
            if (CreateCustomFilterColumnCollection != null) {
                CreateCustomFilterColumnCollection(this, args);
            }
            if (args.FilterColumnCollection != null) {
                return args.FilterColumnCollection;
            }
            return base.CreateFilterColumnCollection();
        }

        protected override void AssignActiveFilterFromFilterBuilder(CriteriaOperator newCriteria) {
            var args = new CustomiseFilterFromFilterBuilderEventArgs(newCriteria);
            if (CustomiseFilterFromFilterBuilder != null) {
                CustomiseFilterFromFilterBuilder(this, args);
            }
            base.AssignActiveFilterFromFilterBuilder(args.Criteria);
        }

        public event EventHandler CancelNewRow;
        public event EventHandler FilterEditorPopup;
        public event EventHandler FilterEditorClosed;
        public event EventHandler<CreateCustomFilterColumnCollectionEventArgs> CreateCustomFilterColumnCollection;
        public event EventHandler<CustomiseFilterFromFilterBuilderEventArgs> CustomiseFilterFromFilterBuilder;
        protected override BaseGridController CreateDataController() {
            if (requireDataControllerType == DataControllerType.AsyncServerMode) {
                _gridController = new AsyncServerModeDataController();
            }
            else {
                if (requireDataControllerType == DataControllerType.ServerMode) {
                    _gridController = new ServerModeDataController();
                }
                else {
                    if (DisableCurrencyManager) {
                        _gridController = new GridDataController();
                    }
                    else {
                        _gridController = new XafCurrencyDataController();
                    }
                }
            }
            return _gridController;
        }

        public void CancelCurrentRowEdit() {
            if ((_gridController != null) && !_gridController.IsDisposed &&
            (ActiveEditor != null) && (_gridController.IsCurrentRowEditing || _gridController.IsCurrentRowModified)) {
                _gridController.CancelCurrentRowEdit();
            }
        }

        public ErrorMessages ErrorMessages {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }
        protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
            if (!SkipMakeRowVisible) {
                base.MakeRowVisibleCore(rowHandle, invalidate);
            }
        }

        public bool SkipMakeRowVisible { get; set; }

        bool ISupportNewItemRow.IsNewItemRowCancelling {
            get { return _isNewItemRowCancelling; }
        }
        protected override void AssignColumns(DevExpress.XtraGrid.Views.Base.ColumnView cv, bool synchronize) {
            base.AssignColumns(cv, synchronize);
            if (!synchronize) {
                ((IModelSynchronizersHolder)this).AssignSynchronizers(cv);
            }
        }
        public object NewItemRowObject {
            get { return null; }
        }

        public event EventHandler RestoreCurrentRow;
        Window IMasterDetailColumnView.Window { get; set; }
        Frame IMasterDetailColumnView.MasterFrame { get; set; }
    }


    public class GridBand : DevExpress.XtraGrid.Views.BandedGrid.GridBand {
        readonly IModelGridBand _modelGridBand;

        public GridBand(IModelGridBand modelGridBand) {
            _modelGridBand = modelGridBand;
        }

        public IModelGridBand ModelGridBand {
            get { return _modelGridBand; }
        }
    }
}
