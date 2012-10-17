using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
    public class AdvBandedListEditor : GridView.XpandGridListEditor {
        public AdvBandedListEditor(IModelListView model)
            : base(model) {
        }

        public new IModelListViewOptionsAdvBandedView Model {
            get { return (IModelListViewOptionsAdvBandedView)base.Model; }
        }
        public new AdvBandedGridView GridView {
            get { return (AdvBandedGridView)base.GridView; }
        }

        protected override IColumnView CreateGridViewCore() {
            return new AdvBandedGridView(this);
        }

        protected override ColumnWrapper CreateGridColumnWrapper(IXafGridColumn xafGridColumn) {
            return new AdvBandedGridColumnWrapper((AdvBandedGridColumn)xafGridColumn);
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new AdvBandedViewLstEditorDynamicModelSynchronizer(this);
        }

        protected override IXafGridColumn CreateGridColumn() {
            return new AdvBandedGridColumn(ObjectTypeInfo, this);
        }
    }
    public class AdvBandedGridView : DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView, IColumnView {
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
                } else {
                    if (RestoreCurrentRow != null) {
                        RestoreCurrentRow(this, EventArgs.Empty);
                    }
                }
            } finally {
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
            string result;
            if (_errorMessages != null) {
                object listItem = GetRow(rowHandle);
                result = column == null ? _errorMessages.GetMessages(listItem) : _errorMessages.GetMessage(column.FieldName, listItem);
            } else {
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
            } else {
                if (requireDataControllerType == DataControllerType.ServerMode) {
                    _gridController = new ServerModeDataController();
                } else {
                    if (DisableCurrencyManager) {
                        _gridController = new GridDataController();
                    } else {
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
            if (_gridListEditor == null) {
                base.AssignColumns(cv, synchronize);
                return;
            }
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                //                if (!((IColumnViewEditor)_gridListEditor).OverrideViewDesignMode) {
                Columns.Clear();
                ////var columnsListEditorModelSynchronizer = new ColumnsListEditorModelSynchronizer(_gridListEditor, _gridListEditor.Model);
                ////columnsListEditorModelSynchronizer.ApplyModel();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<IXafGridColumn>();
                foreach (var column in gridColumns) {

                    var xpandXafGridColumn = column.CreateNew(column.TypeInfo, _gridListEditor);
                    xpandXafGridColumn.ApplyModel(column.Model);
                    Columns.Add((GridColumn)xpandXafGridColumn);
                    xpandXafGridColumn.Assign((GridColumn)column);
                }
                //                } else {
                //                    base.AssignColumns(cv, false);
                //                }
            }
        }

        public object NewItemRowObject {
            get { return null; }
        }

        public event EventHandler RestoreCurrentRow;
        Window IMasterDetailColumnView.Window { get; set; }
        Frame IMasterDetailColumnView.MasterFrame { get; set; }

        bool IColumnView.CanFilterGroupSummaryColumns { get { return false; } set { } }

    }

    public class AdvBandedGridColumnWrapper : XpandGridColumnWrapper {
        public AdvBandedGridColumnWrapper(AdvBandedGridColumn column)
            : base(column) {
        }
    }
    public class AdvBandedGridColumn : BandedGridColumn, IXafGridColumn {
        readonly ITypeInfo _typeInfo;
        readonly AdvBandedListEditor _bandedGridListEditor;
        IModelColumnOptionsAdvBandedView _model;

        public AdvBandedGridColumn(ITypeInfo typeInfo, AdvBandedListEditor bandedGridListEditor) {
            _typeInfo = typeInfo;
            _bandedGridListEditor = bandedGridListEditor;
        }

        public new void Assign(GridColumn gridColumn) {
            base.Assign(gridColumn);
        }

        public bool AllowSummaryChange { get; set; }

        public ColumnsListEditor Editor {
            get { return _bandedGridListEditor; }
        }

        public ITypeInfo TypeInfo {
            get { return _typeInfo; }
        }

        IModelColumn IXafGridColumn.Model {
            get {
                return _model;
            }
        }
        public IModelColumnOptionsAdvBandedView Model {
            get {
                return _model;
            }
        }

        public override Type ColumnType {
            get {
                if (string.IsNullOrEmpty(FieldName) || _typeInfo == null)
                    return base.ColumnType;
                IMemberInfo memberInfo = _typeInfo.FindMember(FieldName);
                return memberInfo != null ? memberInfo.MemberType : base.ColumnType;
            }
        }
        public string PropertyName {
            get {
                return _model != null ? _model.PropertyName : string.Empty;
            }
        }
        private ModelSynchronizer CreateModelSynchronizer() {
            return new ColumnWrapperModelSynchronizer(new AdvBandedGridColumnWrapper(this), _model, _bandedGridListEditor);
        }

        public IXafGridColumn CreateNew(ITypeInfo typeInfo, ColumnsListEditor editor) {
            return new AdvBandedGridColumn(typeInfo, (AdvBandedListEditor)editor);
        }

        public void ApplyModel(IModelColumn columnInfo) {
            _model = (IModelColumnOptionsAdvBandedView)columnInfo;
            CreateModelSynchronizer().ApplyModel();
        }
        public void SynchronizeModel() {
            CreateModelSynchronizer().SynchronizeModel();
        }
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
