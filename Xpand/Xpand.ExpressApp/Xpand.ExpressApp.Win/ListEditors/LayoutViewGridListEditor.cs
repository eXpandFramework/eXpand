using System;
using System.Drawing;
using DevExpress.Xpo;
using DevExpress.Data;
using DevExpress.Utils;
using System.Collections;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Win;
using System.Collections.Generic;
using DevExpress.XtraGrid.Filter;
using DevExpress.ExpressApp.Core;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.ExpressApp.Filtering;
using DevExpress.XtraEditors.Controls;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;

namespace Xpand.ExpressApp.Win.ListEditors {
    public class XafLayoutViewColumn : LayoutViewColumn {
        private ITypeInfo typeInfo;
        private IModelColumn model;
        private LayoutViewListEditor listEditor;
        private ModelSynchronizer CreateModelSynchronizer() {
            return new ColumnWrapperModelSynchronizer(new XafLayoutViewColumnWrapper(this), model, listEditor);
        }
        public XafLayoutViewColumn(ITypeInfo typeInfo, LayoutViewListEditor listEditor) {
            this.typeInfo = typeInfo;
            this.listEditor = listEditor;
        }
        internal new void Assign(GridColumn column) {
            base.Assign(column);
        }
        public void ApplyModel(IModelColumn columnInfo) {
            model = columnInfo;
            CreateModelSynchronizer().ApplyModel();
        }
        public void SynchronizeModel() {
            CreateModelSynchronizer().SynchronizeModel();
        }
        public LayoutViewListEditor ListEditor { get { return listEditor; } }
        public ITypeInfo TypeInfo { get { return typeInfo; } }
        public string PropertyName {
            get {
                if (model != null) {
                    return model.PropertyName;
                }
                return string.Empty;
            }
        }
        public override Type ColumnType {
            get {
                if (string.IsNullOrEmpty(FieldName) || TypeInfo == null) return base.ColumnType;
                IMemberInfo memberInfo = typeInfo.FindMember(FieldName);
                return memberInfo != null ? memberInfo.MemberType : base.ColumnType;
            }
        }
        //Removed
        //private bool allowSummaryChange = true;
        //public bool AllowSummaryChange {
        //    get { return allowSummaryChange; }
        //    set { allowSummaryChange = value; }
        //}
        public IModelColumn Model { get { return model; } }
    }

    public class XafLayoutView : LayoutView {
        private ErrorMessages errorMessages;
        private BaseGridController gridController;
        private Boolean skipMakeRowVisible;
        private Boolean isNewItemRowCancelling;
        private object newItemRowObject;
        internal bool canFilterGroupSummaryColumns = false;
        public XafLayoutView() { }
        public XafLayoutView(GridControl ownerGrid)
            : base(ownerGrid) { }
        internal void SuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
                    //TODO
                    //((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = false;
                }
            }
        }
        internal void CancelSuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
                    //TODO
                    //((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = true;
                }
            }
        }
        private object GetFocusedObject() {
            return MyXtraGridUtils.GetFocusedRowObject(this);
        }
        protected override BaseView CreateInstance() {
            XafLayoutView view = new XafLayoutView();
            view.SetGridControl(GridControl);
            return view;
        }
        protected override void AssignColumns(ColumnView cv, bool synchronize) {
            if (synchronize) {
                base.AssignColumns(cv, synchronize);
            } else {
                Columns.Clear();
                for (int n = 0; n < cv.Columns.Count; n++) {
                    if (cv.Columns[n] is XafLayoutViewColumn) {
                        XafLayoutViewColumn cvColumn = (XafLayoutViewColumn)cv.Columns[n];
                        Columns.Add(new XafLayoutViewColumn(cvColumn.TypeInfo, cvColumn.ListEditor));
                    } else {
                        Columns.Add(new GridColumn());
                    }
                }
                for (int n = 0; n < Columns.Count; n++) {
                    if (Columns[n] is XafLayoutViewColumn) {
                        ((XafLayoutViewColumn)Columns[n]).Assign(cv.Columns[n]);
                    }
                }
            }
        }
        protected override void RaiseShownEditor() {
            if (ActiveEditor is IGridInplaceEdit) {
                if (GetFocusedObject() is IXPSimpleObject) {
                    ((IGridInplaceEdit)ActiveEditor).GridEditingObject = (IXPSimpleObject)GetFocusedObject();
                }
                //Removed
                //if(OptionsView.NewItemRowPosition != DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None && BaseListSourceDataController.NewItemRow == FocusedRowHandle) {
                //    object newObject = this.GetRow(BaseListSourceDataController.NewItemRow);
                //    if(newObject != null) {
                //        if(newObject is IXPSimpleObject) {
                //            ((IGridInplaceEdit)ActiveEditor).GridEditingObject = newObject as IXPSimpleObject;
                //        }
                //    }
                //}
            }
            base.RaiseShownEditor();
        }
        protected override string GetColumnError(int rowHandle, GridColumn column) {
            string result = null;
            if (errorMessages != null) {
                object listItem = GetRow(rowHandle);
                if (column == null) {
                    result = errorMessages.GetMessages(listItem);
                } else {
                    result = errorMessages.GetMessage(column.FieldName, listItem);
                }
            } else {
                result = base.GetColumnError(rowHandle, column);
            }
            return result;
        }
        protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
            return ErrorType.Critical;
        }
        protected virtual void OnCustomCreateFilterColumnCollection(CustomCreateFilterColumnCollectionEventArgs args) {
            if (CustomCreateFilterColumnCollection != null) {
                CustomCreateFilterColumnCollection(this, args);
            }
        }
        protected override FilterColumnCollection CreateFilterColumnCollection() {
            CustomCreateFilterColumnCollectionEventArgs args = new CustomCreateFilterColumnCollectionEventArgs();
            OnCustomCreateFilterColumnCollection(args);
            if (args.FilterColumnCollection == null) {
                args.FilterColumnCollection = base.CreateFilterColumnCollection();
            }
            return args.FilterColumnCollection;
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
        //Removed
        //protected override bool CanBeUsedInGroupSummary(GridColumn column) {
        //    if(canFilterGroupSummaryColumns) {
        //        XafGridColumn xafGridColumn = column as XafGridColumn;
        //        if(xafGridColumn != null && !xafGridColumn.AllowSummaryChange) {
        //            return false;
        //        }
        //    }
        //    return base.CanBeUsedInGroupSummary(column);
        //}
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
            ColumnFilterInfo result = base.DoCustomFilter(column, filterInfo);
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
            gridController = base.CreateDataController();
            return gridController;
        }
        //Removed
        //protected override BaseGridController CreateDataController() {
        //    if(requireDataControllerType == DataControllerType.AsyncServerMode) {
        //        gridController = new AsyncServerModeDataController();
        //    }
        //    else if(requireDataControllerType == DataControllerType.ServerMode) {
        //        gridController = new ServerModeDataController();
        //    }
        //    else if(DisableCurrencyManager) {
        //        gridController = new GridDataController();
        //    }
        //    else {
        //        gridController = new XafCurrencyDataController();
        //    }
        //    return gridController;
        //}
        //protected override RepositoryItem GetFilterRowRepositoryItem(GridColumn column, RepositoryItem current) {
        //    if(column.FilterMode == ColumnFilterMode.Value && current is ILookupEditRepositoryItem) {
        //        return current;
        //    }
        //    return base.GetFilterRowRepositoryItem(column, current);
        //}
        protected override FilterCustomDialog CreateCustomFilterDialog(GridColumn column) {
            if (!OptionsFilter.UseNewCustomFilterDialog) {
                return new XafFilterCustomDialog(column);
            }
            return new XafFilterCustomDialog2(column, Columns);
        }
        protected internal void CancelCurrentRowEdit() {
            if ((gridController != null) && !gridController.IsDisposed
                && (ActiveEditor != null) && (gridController.IsCurrentRowEditing || gridController.IsCurrentRowModified)) {
                gridController.CancelCurrentRowEdit();
            }
        }
        protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
            if (!skipMakeRowVisible) {
                base.MakeRowVisibleCore(rowHandle, invalidate);
            }
        }
        protected internal Boolean SkipMakeRowVisible {
            get { return skipMakeRowVisible; }
            set { skipMakeRowVisible = value; }
        }
        protected internal Boolean IsNewItemRowCancelling {
            get { return isNewItemRowCancelling; }
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
                    FocusedRowHandle = storedFocusedHandle;
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
        internal object NewItemRowObject {
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
        //Removed
        //protected internal new bool FootersIgnoreColumnFormat {
        //    get {
        //        return base.FootersIgnoreColumnFormat;
        //    }
        //}
        public event EventHandler FilterEditorPopup;
        public event EventHandler FilterEditorClosed;
        public event EventHandler CancelNewRow;
        public event EventHandler RestoreCurrentRow;
        public event EventHandler<CustomCreateFilterColumnCollectionEventArgs> CustomCreateFilterColumnCollection;
    }
    public class XafLayoutViewColumnWrapper : ColumnWrapper {
        private const int defaultColumnWidth = 75;
        static DefaultBoolean Convert(bool val) {
            if (!val) {
                return DefaultBoolean.False;
            }
            return DefaultBoolean.Default;
        }
        static bool Convert(DefaultBoolean val) {
            if (val == DefaultBoolean.False) {
                return false;
            }
            return true;
        }
        private XafLayoutViewColumn column;
        public XafLayoutViewColumnWrapper(XafLayoutViewColumn column) {
            this.column = column;
        }
        public XafLayoutViewColumn Column {
            get { return column; }
        }
        public override string Id {
            get {
                return column.Model.Id;
            }
        }
        public override string PropertyName {
            get { return column.PropertyName; }
        }
        public override int SortIndex {
            get { return column.SortIndex; }
            set { column.SortIndex = value; }
        }
        public override ColumnSortOrder SortOrder {
            get { return column.SortOrder; }
            set { column.SortOrder = value; }
        }
        //Removed
        //public override IList<SummaryType> Summary {
        //    get {
        //        IList<SummaryType> list = new List<SummaryType>();
        //        foreach(GridColumnSummaryItem summaryItem in column.Summary) {
        //            list.Add((SummaryType)Enum.Parse(typeof(SummaryType), summaryItem.SummaryType.ToString()));
        //        }
        //        return list;
        //    }
        //    set {
        //        column.Summary.Clear();
        //        if(value != null) {
        //            foreach(SummaryType summaryType in value) {
        //                GridColumnSummaryItem summaryItem = column.Summary.Add((SummaryItemType)Enum.Parse(typeof(SummaryItemType), summaryType.ToString()));
        //                summaryItem.DisplayFormat = summaryItem.GetDefaultDisplayFormat();
        //            }
        //        }
        //    }
        //}
        //public override string SummaryFormat {
        //    get { return column.SummaryItem.DisplayFormat; }
        //    set { column.SummaryItem.DisplayFormat = value; }
        //}
        //public override int GroupIndex {
        //    get { return column.GroupIndex; }
        //    set { column.GroupIndex = value; }
        //}
        //public override DateTimeGroupInterval GroupInterval {
        //    get { return DateTimeGroupIntervalConverter.Convert(column.GroupInterval); }
        //    set { column.GroupInterval = DateTimeGroupIntervalConverter.Convert(value); }
        //}
        //public override bool AllowGroupingChange {
        //    get { return Convert(column.OptionsColumn.AllowGroup); }
        //    set { column.OptionsColumn.AllowGroup = Convert(value); }
        //}
        //public override bool AllowSummaryChange {
        //    get { return column.AllowSummaryChange; }
        //    set { column.AllowSummaryChange = value; }
        //}
        public override bool AllowSortingChange {
            get { return Convert(column.OptionsColumn.AllowSort); }
            set { column.OptionsColumn.AllowSort = Convert(value); }
        }
        public override int VisibleIndex {
            get { return column.VisibleIndex; }
            set { column.VisibleIndex = value; }
        }
        public override string Caption {
            get {
                return column.Caption;
            }
            set {
                column.Caption = value;
                if (string.IsNullOrEmpty(column.Caption)) {
                    column.Caption = column.FieldName;
                }
            }
        }
        public override string DisplayFormat {
            get {
                return column.DisplayFormat.FormatString;
            }
            set {
                column.DisplayFormat.FormatString = value;
                column.DisplayFormat.FormatType = FormatType.Custom;
                column.GroupFormat.FormatString = value;
                column.GroupFormat.FormatType = FormatType.Custom;
            }
        }
        public override int Width {
            get {
                if (column.Width == defaultColumnWidth) {
                    return 0;
                }
                return column.Width;
            }
            set {
                if (value == 0) { return; }
                column.Width = value;
            }
        }
        public override void DisableFeaturesForProtectedContentColumn() {
            base.DisableFeaturesForProtectedContentColumn();
            column.OptionsFilter.AllowFilter = false;
            column.OptionsFilter.AllowAutoFilter = false;
            column.OptionsColumn.AllowIncrementalSearch = false;
        }
        public override void ApplyModel(IModelColumn columnInfo) {
            base.ApplyModel(columnInfo);
            column.ApplyModel(columnInfo);
        }
        public override void SynchronizeModel() {
            base.SynchronizeModel();
            column.SynchronizeModel();
        }
    }
    public class LayoutViewModelSynchronizer : ModelSynchronizer<LayoutView, IModelListView> {
        private LayoutViewListEditor gridListEditor;
        public LayoutViewModelSynchronizer(LayoutViewListEditor gridListEditor, IModelListView model)
            : base(gridListEditor.LayoutView, model) {
            this.gridListEditor = gridListEditor;
            gridListEditor.ControlsCreated += new EventHandler(gridListEditor_ControlsCreated);
        }
        private void gridListEditor_ControlsCreated(object sender, EventArgs e) {
            //Removed
            //Control.OptionsView.ShowFooter = Model.IsFooterVisible;
            //Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
            //Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
            if (gridListEditor.CollectionSource != null) {
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(((IModelListViewWin)Model).ActiveFilterString);
                FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(gridListEditor.CollectionSource.ObjectSpace, Model.ModelClass.TypeInfo, gridListEditor.IsAsyncServerMode());
                criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
                EnumPropertyValueCriteriaProcessor enumParametersProcessor = new EnumPropertyValueCriteriaProcessor(gridListEditor.CollectionSource.ObjectTypeInfo);
                enumParametersProcessor.Process(criteriaOperator);
                Control.ActiveFilterCriteria = criteriaOperator;
            }
            Control.ActiveFilterEnabled = ((IModelListViewWin)Model).IsActiveFilterEnabled;
        }
        protected override void ApplyModelCore() {
            //Removed
            //Control.OptionsBehavior.AutoExpandAllGroups = Model.AutoExpandAllGroups;
            //Control.OptionsView.ShowGroupPanel = Model.IsGroupPanelVisible;
            Control.ActiveFilterEnabled = ((IModelListViewWin)Model).IsActiveFilterEnabled;
            if (gridListEditor.IsAsyncServerMode()) {
                FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(gridListEditor.CollectionSource.ObjectSpace, Model.ModelClass.TypeInfo, true);
                CriteriaOperator criteriaOperator = CriteriaOperator.Parse(((IModelListViewWin)Model).ActiveFilterString);
                criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.StringToObject);
                Control.ActiveFilterCriteria = criteriaOperator;
            } else {
                Control.ActiveFilterString = ((IModelListViewWin)Model).ActiveFilterString;
            }
            if (Model is IModelListViewShowAutoFilterRow) {
                //Removed
                //Control.OptionsView.ShowAutoFilterRow = ((IModelListViewShowAutoFilterRow)Model).ShowAutoFilterRow;
            }
            if (Model is IModelListViewShowFindPanel) {
                if (((IModelListViewShowFindPanel)Model).ShowFindPanel) {
                    Control.ShowFindPanel();
                } else {
                    Control.HideFindPanel();
                }
            }
        }
        public override void SynchronizeModel() {
            //Removed
            //Model.AutoExpandAllGroups = Control.OptionsBehavior.AutoExpandAllGroups;
            //Model.IsGroupPanelVisible = Control.OptionsView.ShowGroupPanel;
            ((IModelListViewWin)Model).IsActiveFilterEnabled = Control.ActiveFilterEnabled;
            if (!Object.ReferenceEquals(Control.ActiveFilterCriteria, null) && gridListEditor.CollectionSource != null) {
                CriteriaOperator criteriaOperator = CriteriaOperator.Clone(Control.ActiveFilterCriteria);
                FilterWithObjectsProcessor criteriaProcessor = new FilterWithObjectsProcessor(gridListEditor.CollectionSource.ObjectSpace);
                criteriaProcessor.Process(criteriaOperator, FilterWithObjectsProcessorMode.ObjectToString);
                ((IModelListViewWin)Model).ActiveFilterString = criteriaOperator.ToString();
            } else {
                ((IModelListViewWin)Model).ActiveFilterString = null;
            }
            if (Model is IModelListViewShowAutoFilterRow) {
                //Removed
                //((IModelListViewShowAutoFilterRow)Model).ShowAutoFilterRow = Control.OptionsView.ShowAutoFilterRow;
            }
            if (Model is IModelListViewShowFindPanel) {
                ((IModelListViewShowFindPanel)Model).ShowFindPanel = Control.IsFindPanelVisible;
            }
        }
        public override void Dispose() {
            base.Dispose();
            if (gridListEditor != null) {
                gridListEditor.ControlsCreated -= new EventHandler(gridListEditor_ControlsCreated);
            }
        }
    }
    public class LayoutGridListEditorSynchronizer : ModelSynchronizer {
        private ModelSynchronizerList modelSynchronizerList;
        public LayoutGridListEditorSynchronizer(LayoutViewListEditor gridListEditor, IModelListView model)
            : base(gridListEditor, model) {
            modelSynchronizerList = new ModelSynchronizerList();
            modelSynchronizerList.Add(new LayoutViewModelSynchronizer(gridListEditor, model));
            modelSynchronizerList.Add(new ColumnsListEditorModelSynchronizer(gridListEditor, model));
            //Removed
            //modelSynchronizerList.Add(new GridSummaryModelSynchronizer(gridListEditor.GridView, model));
            //modelSynchronizerList.Add(new FooterVisibleModelSynchronizer(gridListEditor, model));
            ((LayoutViewListEditor)Control).LayoutView.ColumnPositionChanged += Control_Changed;
        }
        protected override void ApplyModelCore() {
            modelSynchronizerList.ApplyModel();
        }
        public override void SynchronizeModel() {
            modelSynchronizerList.SynchronizeModel();
        }
        public override void Dispose() {
            base.Dispose();
            modelSynchronizerList.Dispose();
            LayoutViewListEditor gridListEditor = Control as LayoutViewListEditor;
            if (gridListEditor != null && gridListEditor.LayoutView != null) {
                gridListEditor.LayoutView.ColumnPositionChanged -= Control_Changed;
            }
        }
    }
    [ListEditor(typeof(object), false)]
    public class LayoutViewListEditor : ColumnsListEditor, /*Removed: ISupportNewItemRowPosition, IGridListEditorTestable, ISupportFooter,*/IControlOrderProvider, IDXPopupMenuHolder, IComplexListEditor, IPrintableSource, ILookupListEditor,
ISupportConditionalFormatting, IHtmlFormattingSupport, IFocusedElementCaptionProvider, ILookupEditProvider, IExportableEditor, ISupportAppearanceCustomization {
        public const string DragEnterCustomCodeId = "DragEnter";
        public const string DragDropCustomCodeId = "DragDrop";
        private RepositoryEditorsFactory repositoryFactory;
        private bool readOnlyEditors = false;
        private GridControl grid;
        private XafLayoutView gridView;
        private int mouseDownTime;
        private int mouseUpTime;
        private bool activatedByMouse;
        private bool focusedChangedRaised;
        private bool selectedChangedRaised;
        private bool isForceSelectRow;
        private int prevFocusedRowHandle;
        private CollectionSourceBase collectionSource;
        private RepositoryItem activeEditor;
        private ActionsDXPopupMenu popupMenu;
        private Boolean processSelectedItemBySingleClick;
        //private Boolean scrollOnMouseMove;
        private Boolean trackMousePosition;
        private TimeLatch moveRowFocusSpeedLimiter;
        private bool selectedItemExecuting;
        //Removed
        //private DevExpress.XtraGrid.Views.Grid.NewItemRowPosition newItemRowPosition = NewItemRowPosition.None;
        private XafApplication application;
        private bool isRowFocusingForced;
        private IPrintable printable;
        //private AppearanceFocusedCellMode appearanceFocusedCellMode = AppearanceFocusedCellMode.Smart;
        private BaseEdit GetEditor(Object sender) {
            if (sender is BaseEdit) {
                return (BaseEdit)sender;
            }
            if (sender is RepositoryItem) {
                return ((RepositoryItem)sender).OwnerEdit;
            }
            return null;
        }
        //Removed
        //private void SetNewItemRow() {
        //    if(gridView == null) {
        //        return;
        //    }
        //    gridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
        //    gridView.CancelNewRow -= new EventHandler(gridView_CancelNewRow);
        //    if(gridView.DataController is XafCurrencyDataController) {
        //        ((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding -= new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
        //    }
        //    gridView.OptionsView.NewItemRowPosition = (DevExpress.XtraGrid.Views.Grid.NewItemRowPosition)Enum.Parse(typeof(DevExpress.XtraGrid.Views.Grid.NewItemRowPosition), newItemRowPosition.ToString());
        //    if (newItemRowPosition != NewItemRowPosition.None) {
        //        gridView.InitNewRow += new InitNewRowEventHandler(gridView_InitNewRow);
        //        gridView.CancelNewRow += new EventHandler(gridView_CancelNewRow);
        //        if (gridView.DataController is XafCurrencyDataController) {
        //            ((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding += new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
        //        }
        //    }
        //}
        private void SubscribeLayoutViewEvents() {
            gridView.BeforeLeaveRow += new RowAllowEventHandler(gridView_BeforeLeaveRow);
            gridView.FocusedRowChanged += new FocusedRowChangedEventHandler(gridView_FocusedRowChanged);
            gridView.ColumnFilterChanged += new EventHandler(gridView_ColumnFilterChanged);
            gridView.SelectionChanged += new SelectionChangedEventHandler(gridView_SelectionChanged);
            gridView.ShowingEditor += new CancelEventHandler(gridView_EditorShowing);
            gridView.ShownEditor += new EventHandler(gridView_ShownEditor);
            gridView.HiddenEditor += new EventHandler(gridView_HiddenEditor);
            gridView.MouseDown += new MouseEventHandler(gridView_MouseDown);
            gridView.MouseUp += new MouseEventHandler(gridView_MouseUp);
            gridView.Click += new EventHandler(gridView_Click);
            //Removed
            //gridView.MouseMove += new MouseEventHandler(gridView_MouseMove);
            gridView.MouseWheel += new MouseEventHandler(gridView_MouseWheel);
            gridView.ShowCustomization += new EventHandler(gridView_ShowCustomizationForm);
            gridView.HideCustomization += new EventHandler(gridView_HideCustomizationForm);
            gridView.CustomFieldValueStyle += new DevExpress.XtraGrid.Views.Layout.Events.LayoutViewFieldValueStyleEventHandler(gridView_RowCellStyle);
            //Removed
            //gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
            //gridView.ColumnChanged += new EventHandler(gridView_ColumnChanged);
            gridView.FocusedRowLoaded += new RowEventHandler(gridView_FocusedRowLoaded);
            gridView.CustomCreateFilterColumnCollection += new EventHandler<CustomCreateFilterColumnCollectionEventArgs>(gridView_CustomCreateFilterColumnCollection);
            gridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Layout.Events.LayoutViewCustomRowCellEditEventHandler(gridView_CustomRowCellEdit);
            gridView.FilterEditorPopup += new EventHandler(gridView_FilterEditorPopup);
            gridView.FilterEditorClosed += new EventHandler(gridView_FilterEditorClosed);
            if (AllowEdit) {
                gridView.ValidateRow += new ValidateRowEventHandler(gridView_ValidateRow);
            }
            if (gridView.DataController != null) {
                gridView.DataController.ListChanged += new ListChangedEventHandler(DataController_ListChanged);
            }
        }
        private void gridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            if (e.RowHandle == GridControl.AutoFilterRowHandle && e.Column.OptionsFilter.FilterBySortField != DefaultBoolean.False && !String.IsNullOrEmpty(e.Column.FieldNameSortGroup) && e.Column.FieldName != e.Column.FieldNameSortGroup) {
                e.RepositoryItem = new RepositoryItemStringEdit();
            }
        }
        private void gridView_CustomCreateFilterColumnCollection(object sender, CustomCreateFilterColumnCollectionEventArgs e) {
            if (collectionSource != null) {
                IFilterColumnCollectionHelper helper = new FilterColumnCollectionHelper(application, collectionSource.ObjectSpace, collectionSource.ObjectTypeInfo);
                e.FilterColumnCollection = new MemberInfoFilterColumnCollection(helper);
            }
        }
        //Removed
        //protected virtual void CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
        //    if(e.MenuType == GridMenuType.Summary) {
        //        MyXafGridColumn xafGridColumn = e.HitInfo.Column as MyXafGridColumn;
        //        if(xafGridColumn != null) {
        //            e.Allow = xafGridColumn.AllowSummaryChange;
        //        }
        //    }
        //}
        //void IGridListEditorTestable.CustomizeGridMenu(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
        //    CustomizeGridMenu(e);
        //}
        private void UnsubscribeGridViewEvents() {
            gridView.FocusedRowChanged -= new FocusedRowChangedEventHandler(gridView_FocusedRowChanged);
            gridView.ColumnFilterChanged -= new EventHandler(gridView_ColumnFilterChanged);
            gridView.SelectionChanged -= new SelectionChangedEventHandler(gridView_SelectionChanged);
            gridView.ShowingEditor -= new CancelEventHandler(gridView_EditorShowing);
            gridView.ShownEditor -= new EventHandler(gridView_ShownEditor);
            gridView.HiddenEditor -= new EventHandler(gridView_HiddenEditor);
            gridView.MouseDown -= new MouseEventHandler(gridView_MouseDown);
            gridView.MouseUp -= new MouseEventHandler(gridView_MouseUp);
            gridView.Click -= new EventHandler(gridView_Click);
            //Removed
            //gridView.MouseMove -= new MouseEventHandler(gridView_MouseMove);
            gridView.MouseWheel -= new MouseEventHandler(gridView_MouseWheel);
            gridView.ShowCustomization -= new EventHandler(gridView_ShowCustomizationForm);
            gridView.HideCustomization -= new EventHandler(gridView_HideCustomizationForm);
            gridView.CustomFieldValueStyle -= new DevExpress.XtraGrid.Views.Layout.Events.LayoutViewFieldValueStyleEventHandler(gridView_RowCellStyle);
            gridView.ValidateRow -= new ValidateRowEventHandler(gridView_ValidateRow);
            gridView.BeforeLeaveRow -= new RowAllowEventHandler(gridView_BeforeLeaveRow);
            //Removed
            //gridView.PopupMenuShowing -= new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_ShowGridMenu);
            //gridView.ColumnChanged -= new EventHandler(gridView_ColumnChanged);
            gridView.FocusedRowLoaded -= new RowEventHandler(gridView_FocusedRowLoaded);
            gridView.CustomCreateFilterColumnCollection -= new EventHandler<CustomCreateFilterColumnCollectionEventArgs>(gridView_CustomCreateFilterColumnCollection);
            gridView.CustomRowCellEdit -= new DevExpress.XtraGrid.Views.Layout.Events.LayoutViewCustomRowCellEditEventHandler(gridView_CustomRowCellEdit);
            gridView.FilterEditorPopup -= new EventHandler(gridView_FilterEditorPopup);
            gridView.FilterEditorClosed -= new EventHandler(gridView_FilterEditorClosed);
            if (gridView.DataController != null) {
                gridView.DataController.ListChanged -= new ListChangedEventHandler(DataController_ListChanged);
            }
        }
        private void SetGridViewOptions() {
            gridView.OptionsBehavior.EditorShowMode = EditorShowMode.Click;
            gridView.OptionsBehavior.Editable = true;
            //Removed
            //gridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
            gridView.OptionsBehavior.AutoPopulateColumns = false;
            gridView.OptionsBehavior.FocusLeaveOnTab = true;
            gridView.OptionsSelection.MultiSelect = true;
            //Removed
            //gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
            //gridView.OptionsNavigation.AutoFocusNewRow = true;
            //gridView.OptionsNavigation.AutoMoveRowFocus = true;
            //gridView.OptionsView.ShowDetailButtons = false;
            //gridView.OptionsDetail.EnableMasterViewMode = false;
            //gridView.OptionsView.ShowIndicator = true;
            gridView.OptionsFilter.FilterEditorAggregateEditing = FilterControlAllowAggregateEditing.AggregateWithCondition;
            gridView.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
            //Removed
            //gridView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            gridView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            ApplyHtmlFormatting();
            //Removed
            //gridView.OptionsMenu.ShowGroupSummaryEditorItem = true;
        }
        private void SetupLayoutView() {
            if (gridView == null) {
                throw new ArgumentNullException("gridView");
            }
            gridView.TemplateCard = new LayoutViewCard();
            gridView.CardMinSize = new Size(400, 200);
            gridView.ErrorMessages = ErrorMessages;
            gridView.OptionsBehavior.AutoPopulateColumns = false;
            gridView.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
            //gridView.OptionsBehavior.Editable = true;
            //gridView.OptionsBehavior.AutoSelectAllInEditor = false;
            //gridView.OptionsBehavior.FocusLeaveOnTab = true;
            gridView.OptionsSelection.MultiSelect = SelectionType == SelectionType.MultipleSelection || SelectionType == SelectionType.Full;
            gridView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;

            SubscribeLayoutViewEvents();
            //Removed
            //SetNewItemRow();
        }
        private LayoutView CreateLayoutView() {
            gridView = CreateLayoutViewCore();
            return gridView;
        }
        private void SelectRowByHandle(int rowHandle) {
            if (gridView.IsValidRowHandle(rowHandle)) {
                try {
                    isRowFocusingForced = true;
                    MyXtraGridUtils.SelectRowByHandle(gridView, rowHandle);
                } finally {
                    isRowFocusingForced = false;
                }
            }
        }
        private void gridView_DataController_NewItemRowObjectAdding(object sender, HandledEventArgs e) {
            e.Handled = OnNewObjectAdding() != null;
        }
        private void gridView_InitNewRow(object sender, InitNewRowEventArgs e) {
            OnNewObjectCreated();
        }
        private void gridView_CancelNewRow(object sender, EventArgs e) {
            OnNewObjectCanceled();
        }
        //private void gridView_ShowGridMenu(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e) {
        //    CustomizeGridMenu(e);
        //}
        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Layout.Events.LayoutViewFieldValueStyleEventArgs e) {
            if (e.RowHandle != GridControl.AutoFilterRowHandle) {
                string propertyName = e.Column is XafLayoutViewColumn ? ((XafLayoutViewColumn)e.Column).PropertyName : e.Column.FieldName;
                OnCustomizeAppearance(new CustomizeAppearanceEventArgs(propertyName, new AppearanceObjectAdapter(e.Appearance, e), e.RowHandle));
                if (ObjectConditionalFormatting != null) {
                    object rowObj = MyXtraGridUtils.GetRow(CollectionSource, sender as LayoutView, e.RowHandle);
                    ObjectConditionalFormattingEventArgs eventArgs = new ObjectConditionalFormattingEventArgs(rowObj, propertyName);
                    ObjectConditionalFormatting(this, eventArgs);
                    if (eventArgs.ColorSet.IsTargetHighlighted(ColorHighlightingTarget.Foreground)) {
                        e.Appearance.ForeColor = eventArgs.ColorSet.GetTargetColor(ColorHighlightingTarget.Foreground);
                    }
                    if (eventArgs.ColorSet.IsTargetHighlighted(ColorHighlightingTarget.Background)) {
                        e.Appearance.BackColor = eventArgs.ColorSet.GetTargetColor(ColorHighlightingTarget.Background);
                    }
                }
            }
        }
        private void gridView_MouseWheel(object sender, MouseEventArgs e) {
            moveRowFocusSpeedLimiter.Reset();
        }
        private void gridView_HideCustomizationForm(object sender, EventArgs e) {
            OnEndCustomization();
        }
        //TODO
        private void gridView_ShowCustomizationForm(object sender, EventArgs e) {
            OnBeginCustomization();
        }
        private void gridView_FilterEditorPopup(object sender, EventArgs e) {
            OnBeginCustomization();
        }
        private void gridView_FilterEditorClosed(object sender, EventArgs e) {
            OnEndCustomization();
        }
        private void OnBeginCustomization() {
            if (BeginCustomization != null) {
                BeginCustomization(this, EventArgs.Empty);
            }
        }
        private void OnEndCustomization() {
            if (EndCustomization != null) {
                EndCustomization(this, EventArgs.Empty);
            }
        }
        private bool IsGroupRowHandle(int handle) {
            return handle < 0;
        }
        private void grid_HandleCreated(object sender, EventArgs e) {
            AssignDataSourceToControl(DataSource);
        }
        private void grid_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            ProcessGridKeyDown(e);
        }
        private void SubmitActiveEditorChanges() {
            if ((LayoutView.ActiveEditor != null) && LayoutView.ActiveEditor.IsModified) {
                LayoutView.PostEditor();
                LayoutView.UpdateCurrentRow();
            }
        }
        private void grid_DoubleClick(object sender, EventArgs e) {
            ProcessMouseClick(e);
        }
        private void gridView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            isForceSelectRow = e.Action == CollectionChangeAction.Add;
            OnSelectionChanged();
        }
        private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            if (!isRowFocusingForced && DataSource != null && !(DataSource is XPBaseCollection)) {
                prevFocusedRowHandle = e.PrevFocusedRowHandle;
            }
            OnFocusedObjectChanged();
        }
        private void gridView_FocusedRowLoaded(object sender, RowEventArgs e) {
            if (IsAsyncServerMode()) {
                OnFocusedObjectChanged();
            }
        }
        private void gridView_ColumnFilterChanged(object sender, EventArgs e) {
            if (!LayoutView.IsLoading) {
                OnFocusedObjectChanged();
            }
        }
        private void gridView_Click(object sender, EventArgs e) {
            if (processSelectedItemBySingleClick) {
                ProcessMouseClick(e);
            }
        }
        //Removed
        //private Boolean IsLastVisibleRow(GridHitInfo hitInfo, RowVisibleState rowVisibleState) {
        //    Boolean result = false;
        //    if(hitInfo.RowHandle >= 0) {
        //        if(rowVisibleState == RowVisibleState.Partially) {
        //            result = true;
        //        }
        //        else if(rowVisibleState == RowVisibleState.Visible) {
        //            if(hitInfo.RowHandle < gridView.RowCount - 1) {
        //                RowVisibleState nextRowVisibleState = gridView.IsRowVisible(hitInfo.RowHandle + 1);
        //                if(nextRowVisibleState == RowVisibleState.Hidden) {
        //                    result = true;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}
        //private void gridView_MouseMove(object sender, MouseEventArgs e) {
        //    if(trackMousePosition || scrollOnMouseMove) {
        //        GridHitInfo hitInfo = gridView.CalcHitInfo(e.Location);
        //        if(hitInfo.InRow) {
        //            Boolean isTimeIntervalExpired = moveRowFocusSpeedLimiter.IsTimeIntervalExpired;
        //            if(isTimeIntervalExpired) {
        //                moveRowFocusSpeedLimiter.ResetLastEventTime();
        //            }
        //            RowVisibleState rowVisibleState = gridView.IsRowVisible(hitInfo.RowHandle);
        //            if(hitInfo.RowHandle == gridView.TopRowIndex) {
        //                if(scrollOnMouseMove && isTimeIntervalExpired && (gridView.TopRowIndex != 0)) {
        //                    if(trackMousePosition) {
        //                        MyXtraGridUtils.SelectRowByHandle(gridView, gridView.TopRowIndex - 1);
        //                    }
        //                    gridView.TopRowIndex--;
        //                }
        //                else if(trackMousePosition && (gridView.FocusedRowHandle != gridView.TopRowIndex)) {
        //                    MyXtraGridUtils.SelectRowByHandle(gridView, gridView.TopRowIndex);
        //                }
        //            }
        //            else if(IsLastVisibleRow(hitInfo, rowVisibleState)) {
        //                if(scrollOnMouseMove && isTimeIntervalExpired && (hitInfo.RowHandle < gridView.RowCount - 1)) {
        //                    gridView.TopRowIndex++;
        //                    if(trackMousePosition) {
        //                        if(rowVisibleState == RowVisibleState.Partially) {
        //                            MyXtraGridUtils.SelectRowByHandle(gridView, hitInfo.RowHandle);
        //                        }
        //                        else {
        //                            MyXtraGridUtils.SelectRowByHandle(gridView, hitInfo.RowHandle + 1);
        //                        }
        //                    }
        //                }
        //                else if(trackMousePosition && (gridView.FocusedRowHandle != hitInfo.RowHandle)) {
        //                    if(rowVisibleState == RowVisibleState.Visible) {
        //                        MyXtraGridUtils.SelectRowByHandle(gridView, hitInfo.RowHandle);
        //                    }
        //                    else if(rowVisibleState == RowVisibleState.Partially) {
        //                        gridView.SkipMakeRowVisible = true;
        //                        try {
        //                            MyXtraGridUtils.SelectRowByHandle(gridView, hitInfo.RowHandle);
        //                        }
        //                        finally {
        //                            gridView.SkipMakeRowVisible = false;
        //                        }
        //                    }
        //                }
        //            }
        //            else {
        //                if(trackMousePosition && (gridView.FocusedRowHandle != hitInfo.RowHandle)) {
        //                    MyXtraGridUtils.SelectRowByHandle(gridView, hitInfo.RowHandle);
        //                }
        //            }
        //        }
        //    }
        //}
        private void gridView_ValidateRow(object sender, ValidateRowEventArgs e) {
            if (e.Valid) {
                ValidateObjectEventArgs ea = new ValidateObjectEventArgs(FocusedObject, true);
                OnValidateObject(ea);
                e.Valid = ea.Valid;
                e.ErrorText = ea.ErrorText;
            }
        }
        private void gridView_BeforeLeaveRow(object sender, RowAllowEventArgs e) {
            if (e.Allow) {
                if (!gridView.IsNewItemRowCancelling) {
                    e.Allow = OnFocusedObjectChanging();
                }
            }
        }
        private void gridView_EditorShowing(object sender, CancelEventArgs e) {
            activeEditor = null;
            string propertyName = gridView.FocusedColumn.FieldName;
            RepositoryItem edit = gridView.FocusedColumn.ColumnEdit;
            OnCustomizeAppearance(new CustomizeAppearanceEventArgs(propertyName, new CancelEventArgsAppearanceAdapter(e), gridView.FocusedRowHandle));
            if (!e.Cancel) {
                if (edit != null) {
                    OnCustomizeAppearance(new CustomizeAppearanceEventArgs(propertyName, new AppearanceObjectAdapterWithReset(edit.Appearance, edit), gridView.FocusedRowHandle));
                    edit.MouseDown += new MouseEventHandler(Editor_MouseDown);
                    edit.MouseUp += new MouseEventHandler(Editor_MouseUp);
                    RepositoryItemButtonEdit buttonEdit = edit as RepositoryItemButtonEdit;
                    if (buttonEdit != null) {
                        buttonEdit.ButtonPressed += new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
                    }
                    RepositoryItemBaseSpinEdit spinEdit = edit as RepositoryItemBaseSpinEdit;
                    if (spinEdit != null) {
                        spinEdit.Spin += new SpinEventHandler(SpinEdit_Spin);
                    }
                    edit.KeyDown += new KeyEventHandler(Editor_KeyDown);
                    activeEditor = edit;
                }
            }
        }
        private void gridView_ShownEditor(object sender, EventArgs e) {
            if (popupMenu != null) {
                popupMenu.ResetPopupContextMenuSite();
            }
            PopupBaseEdit popupEdit = gridView.ActiveEditor as PopupBaseEdit;
            if (popupEdit != null && activatedByMouse && popupEdit.Properties.ShowDropDown != ShowDropDown.Never) {
                popupEdit.ShowPopup();
            }
            activatedByMouse = false;
            LookupEdit editor = gridView.ActiveEditor as LookupEdit;
            if (editor != null) {
                OnLookupEditCreated(editor);
            }
        }
        private void gridView_HiddenEditor(object sender, EventArgs e) {
            if (popupMenu != null) {
                popupMenu.SetupPopupContextMenuSite();
            }
            LookupEdit editor = gridView.ActiveEditor as LookupEdit;
            if (editor != null) {
                OnLookupEditCreated(editor);
            }
            if (activeEditor != null) {
                activeEditor.KeyDown -= new KeyEventHandler(Editor_KeyDown);
                activeEditor.MouseDown -= new MouseEventHandler(Editor_MouseDown);
                activeEditor.MouseUp -= new MouseEventHandler(Editor_MouseUp);
                RepositoryItemButtonEdit buttonEdit = activeEditor as RepositoryItemButtonEdit;
                if (buttonEdit != null) {
                    buttonEdit.ButtonPressed -= new ButtonPressedEventHandler(ButtonEdit_ButtonPressed);
                }
                RepositoryItemBaseSpinEdit spinEdit = activeEditor as RepositoryItemBaseSpinEdit;
                if (spinEdit != null) {
                    spinEdit.Spin -= new SpinEventHandler(SpinEdit_Spin);
                }
                activeEditor = null;
            }
        }
        private void gridView_MouseDown(object sender, MouseEventArgs e) {
            LayoutView view = (LayoutView)sender;
            LayoutViewHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
            if (hi.RowHandle >= 0) {
                mouseDownTime = System.Environment.TickCount;
            } else {
                mouseDownTime = 0;
            }
            activatedByMouse = true;
        }
        private void gridView_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = System.Environment.TickCount;
        }
        //Removed
        //private void UpdateAppearanceFocusedCell() {
        //    if(gridView != null) {
        //        switch(AppearanceFocusedCellMode) {
        //            case AppearanceFocusedCellMode.Smart:
        //                gridView.OptionsSelection.EnableAppearanceFocusedCell = LayoutView.VisibleColumns.Count > 1;
        //                break;
        //            case AppearanceFocusedCellMode.Enabled:
        //                gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
        //                break;
        //            case AppearanceFocusedCellMode.Disabled:
        //                gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
        //                break;
        //        }
        //    }
        //}
        //private void gridView_ColumnChanged(object sender, EventArgs e) {
        //    UpdateAppearanceFocusedCell();
        //}
        private void Editor_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Int32 currentTime = System.Environment.TickCount;
                if ((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < SystemInformation.DoubleClickTime)) {
                    OnProcessSelectedItem();
                    mouseDownTime = 0;
                }
            }
        }
        private void Editor_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = System.Environment.TickCount;
        }
        private void Editor_KeyDown(object sender, KeyEventArgs e) {
            ProcessEditorKeyDown(e);
        }
        private void SpinEdit_Spin(object sender, SpinEventArgs e) {
            mouseDownTime = 0;
        }
        private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e) {
            mouseDownTime = 0;
        }
        private void DataController_ListChanged(object sender, ListChangedEventArgs e) {
            if ((grid != null) && (grid.FindForm() != null) && !grid.ContainsFocus) {
                IList dataSource = ListHelper.GetList(((BaseGridController)sender).DataSource);
                if (dataSource != null && dataSource.Count == 1 && e.ListChangedType == ListChangedType.ItemAdded) {
                    IEditableObject obj = dataSource[e.NewIndex] as IEditableObject;
                    if (obj != null) {
                        obj.EndEdit();
                    }
                }
            }
            if (e.ListChangedType == ListChangedType.ItemChanged) {
                prevFocusedRowHandle = gridView.FocusedRowHandle;
            }
            if (e.ListChangedType == ListChangedType.ItemDeleted && gridView.FocusedRowHandle != BaseListSourceDataController.NewItemRow) {
                if (!gridView.IsValidRowHandle(prevFocusedRowHandle)) {
                    prevFocusedRowHandle--;
                }
                SelectRowByHandle(prevFocusedRowHandle);
                OnFocusedObjectChanged();
            }
            if (gridView != null) {
                if (e.ListChangedType == ListChangedType.Reset) {
                    if (gridView.IsServerMode) {
                        SelectRowByHandle(prevFocusedRowHandle);
                    }
                    if (gridView.SelectedRowsCount == 0) {
                        MyXtraGridUtils.SelectFocusedRow(gridView);
                    }
                    OnFocusedObjectChanged();
                }
            }
        }
        private void SetTag() {
            if (grid != null) {
                grid.Tag = EasyTestTagHelper.FormatTestTable(Name);
            }
        }
        private void repositoryItem_EditValueChanging(object sender, ChangingEventArgs e) {
            BaseEdit editor = GetEditor(sender);
            if ((editor != null) && (editor.InplaceType == InplaceType.Grid)) {
                OnObjectChanged();
            }
        }
        private void OnPrintableChanged() {
            if (PrintableChanged != null) {
                PrintableChanged(this, new PrintableChangedEventArgs(printable));
            }
        }
        private void grid_VisibleChanged(object sender, EventArgs e) {
            AssignDataSourceToControl(DataSource);
            if (grid.Visible) {
                GridColumn defaultColumn = GetDefaultColumn();
                if (defaultColumn != null)
                    gridView.FocusedColumn = defaultColumn;
            }
        }
        private void grid_Paint(object sender, PaintEventArgs e) {
            grid.Paint -= new PaintEventHandler(grid_Paint);
            //Removed
            //UpdateAppearanceFocusedCell();
        }
        private void grid_ParentChanged(object sender, EventArgs e) {
            if (grid.Parent != null) {
                grid.ForceInitialize();
            }
        }
        private GridColumn GetDefaultColumn() {
            GridColumn result = null;
            if (Model != null) {
                ITypeInfo classType = Model.ModelClass.TypeInfo;
                if (classType != null) {
                    IMemberInfo defaultMember = classType.DefaultMember;
                    if (defaultMember != null) {
                        result = LayoutView.Columns[defaultMember.Name];
                    }
                }
            }
            return result == null || !result.Visible ? null : result;
        }
        private void RemoveColumnInfo(GridColumn column) {
            if (column is XafLayoutViewColumn) {
                IModelColumn columnInfo = Model.Columns[((XafLayoutViewColumn)column).Model.Id];
                if (columnInfo != null) {
                    Model.Columns.Remove(columnInfo);
                }
            }
        }
        private void UpdateAllowEditGridViewAndColumnsOptions() {
            if (gridView != null) {
                gridView.BeginUpdate();
                foreach (GridColumn column in gridView.Columns) {
                    column.OptionsColumn.AllowEdit = column.ColumnEdit != null && IsDataShownOnDropDownWindow(column.ColumnEdit) ? true : AllowEdit;
                    if (column.ColumnEdit != null) {
                        column.ColumnEdit.ReadOnly = !AllowEdit || ReadOnlyEditors;
                    }
                    if (AllowEdit) {
                        gridView.ValidateRow += new ValidateRowEventHandler(gridView_ValidateRow);
                    } else {
                        gridView.ValidateRow -= new ValidateRowEventHandler(gridView_ValidateRow);
                    }
                    //Removed
                    //gridView.OptionsBehavior.AllowIncrementalSearch = !AllowEdit || ReadOnlyEditors;
                }
                gridView.EndUpdate();
            }
        }
        internal bool IsAsyncServerMode() {
            CollectionSource source = CollectionSource as CollectionSource;
            return ((source != null) && source.IsServerMode && source.IsAsyncServerMode);
        }
        private bool IsReplacedColumnByAsyncServerMode(string propertyName) {
            IMemberInfo memberInfo = ObjectTypeInfo.FindMember(propertyName);
            return IsAsyncServerMode()
                && (memberInfo != null)
                && (memberInfo.MemberType != typeof(Type))
                && (GetDisplayablePropertyName(propertyName) != memberInfo.BindingName)
                && SimpleTypes.IsClass(memberInfo.MemberType);
        }
        private IMemberInfo FindMemberInfoForColumn(IModelColumn columnInfo) {
            if (IsReplacedColumnByAsyncServerMode(columnInfo.PropertyName)) {
                return ObjectTypeInfo.FindMember(GetDisplayablePropertyName(columnInfo.PropertyName));
            }
            return ObjectTypeInfo.FindMember(columnInfo.PropertyName);
        }
        protected virtual void OnCustomizeAppearance(CustomizeAppearanceEventArgs args) {
            if (CustomizeAppearance != null) {
                CustomizeAppearanceEventArgs workArgs;
                bool canCustomizeAppearance = true;
                if (!IsAsyncServerMode()) {
                    object rowObj = MyXtraGridUtils.GetRow(gridView, (int)args.ContextObject);
                    workArgs = new CustomizeAppearanceEventArgs(args.Name, args.Item, rowObj);
                } else {
                    AsyncServerModeContextDescriptor contextDescriptor = new AsyncServerModeContextDescriptor(gridView, CollectionSource.ObjectSpace, CollectionSource.ObjectTypeInfo.Type);
                    workArgs = new CustomizeAppearanceEventArgs(args.Name, args.Item, args.ContextObject, contextDescriptor);
                    canCustomizeAppearance = gridView.IsRowLoaded((int)args.ContextObject);
                }
                if (canCustomizeAppearance) {
                    CustomizeAppearance(this, workArgs);
                }
            }
        }
        protected virtual XafLayoutView CreateLayoutViewCore() {
            return new XafLayoutView();
        }
        protected virtual void ProcessMouseClick(EventArgs e) {
            if (!selectedItemExecuting) {
                if (LayoutView.FocusedRowHandle >= 0) {
                    DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(grid, e);
                    LayoutViewHitInfo hitInfo = LayoutView.CalcHitInfo(args.Location);
                    if (hitInfo.InCard && (hitInfo.HitTest == LayoutViewHitTest.Field)) {
                        args.Handled = true;
                        this.OnProcessSelectedItem();
                    }
                }
            }
        }
        protected virtual void ProcessGridKeyDown(System.Windows.Forms.KeyEventArgs e) {
            if (FocusedObject != null && e.KeyCode == Keys.Enter) {
                if (LayoutView.ActiveEditor == null && !ReadOnlyEditors) {
                    OnProcessSelectedItem();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                } else {
                    if (ReadOnlyEditors && LayoutView.ActiveEditor == null) {
                        if (gridView.IsLastColumnFocused) {
                            gridView.UpdateCurrentRow();
                            e.Handled = true;
                        } else {
                            LayoutView.FocusedColumn =
                                LayoutView.GetVisibleColumn(1 + gridView.VisibleColumns.IndexOf(LayoutView.FocusedColumn));
                            e.Handled = true;
                        }
                    } else {
                        PopupBaseEdit popupEdit = LayoutView.ActiveEditor as PopupBaseEdit;
                        if ((popupEdit == null) || (!popupEdit.IsPopupOpen)) {
                            SubmitActiveEditorChanges();
                            e.Handled = true;
                        }
                    }
                }
            }
        }
        protected virtual void ProcessEditorKeyDown(KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                SubmitActiveEditorChanges();
            }
        }
        protected virtual void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
            if (ColumnCreated != null) {
                ColumnCreatedEventArgs args = new ColumnCreatedEventArgs(column, columnInfo);
                ColumnCreated(this, args);
            }
        }
        protected override void OnFocusedObjectChanged() {
            base.OnFocusedObjectChanged();
            focusedChangedRaised = true;
        }
        protected override void OnSelectionChanged() {
            base.OnSelectionChanged();
            selectedChangedRaised = true;
            if (LayoutView.SelectedRowsCount == 0 && isForceSelectRow) {
                MyXtraGridUtils.SelectFocusedRow(LayoutView);
            }
        }
        protected virtual void OnGridDataSourceChanging() {
            if (GridDataSourceChanging != null) {
                GridDataSourceChanging(this, EventArgs.Empty);
            }
        }
        protected override object CreateControlsCore() {
            if (grid == null) {
                grid = new GridControl();
                ((System.ComponentModel.ISupportInitialize)(grid)).BeginInit();
                try {
                    grid.MinimumSize = new Size(100, 75);
                    grid.Dock = DockStyle.Fill;
                    grid.AllowDrop = true;
                    grid.HandleCreated += new EventHandler(grid_HandleCreated);
                    grid.KeyDown += new KeyEventHandler(grid_KeyDown);
                    grid.DoubleClick += new EventHandler(grid_DoubleClick);
                    grid.ParentChanged += new EventHandler(grid_ParentChanged);
                    grid.Paint += new PaintEventHandler(grid_Paint);
                    grid.VisibleChanged += new EventHandler(grid_VisibleChanged);
                    grid.Height = 100;
                    grid.TabStop = true;
                    grid.MainView = CreateLayoutView();
                    SetupLayoutView();
                    SetGridViewOptions();
                    ApplyModel();
                    SetTag();
                } finally {
                    ((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
                    grid.ForceInitialize();
                }
                Printable = Grid;
            }
            return grid;
        }
        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new LayoutGridListEditorSynchronizer(this, Model);
        }

        public override void ApplyModel() {
            Grid.BeginUpdate();
            LayoutView.BeginInit();
            try {
                base.ApplyModel();
                //Removed
                //UpdateAppearanceFocusedCell();
            } finally {
                LayoutView.EndInit();
                Grid.EndUpdate();
            }
        }
        protected override void OnProcessSelectedItem() {
            selectedItemExecuting = true;
            try {
                if ((gridView != null) && (gridView.ActiveEditor != null)) {
                    BindingHelper.EndCurrentEdit(Grid);
                }
                base.OnProcessSelectedItem();
            } finally {
                selectedItemExecuting = false;
            }
        }
        protected internal bool IsDataShownOnDropDownWindow(RepositoryItem repositoryItem) {
            return DXPropertyEditor.RepositoryItemsTypesWithMandatoryButtons.Contains(repositoryItem.GetType());
        }
        protected override void AssignDataSourceToControl(Object dataSource) {
            if (grid != null && grid.DataSource != dataSource) {
                if (grid.IsHandleCreated && grid.Visible) {
                    focusedChangedRaised = false;
                    selectedChangedRaised = false;
                    OnGridDataSourceChanging();
                    grid.BeginUpdate();
                    try {
                        if (gridView.DataController != null) {
                            gridView.DataController.ListChanged -= new ListChangedEventHandler(DataController_ListChanged);
                        }
                        gridView.CancelCurrentRowEdit();
                        grid.DataSource = dataSource;
                        if (gridView.DataController != null) {
                            gridView.DataController.ListChanged += new ListChangedEventHandler(DataController_ListChanged);
                        }
                    } finally {
                        grid.EndUpdate();
                    }
                    if (!selectedChangedRaised) {
                        OnSelectionChanged();
                    }
                    if (!focusedChangedRaised) {
                        OnFocusedObjectChanged();
                    }
                } else {
                    grid.DataSource = null;
                }
            }
        }
        protected override void OnProtectedContentTextChanged() {
            base.OnProtectedContentTextChanged();
            repositoryFactory.ProtectedContentText = ProtectedContentText;
        }
        protected override void OnAllowEditChanged() {
            UpdateAllowEditGridViewAndColumnsOptions();
            base.OnAllowEditChanged();
        }
        protected override void OnErrorMessagesChanged() {
            base.OnErrorMessagesChanged();
            if (grid != null && gridView != null) {
                grid.Refresh();
                gridView.LayoutChanged();
            }
        }
        public LayoutViewListEditor(IModelListView model)
            : base(model) {
            popupMenu = new ActionsDXPopupMenu();
            moveRowFocusSpeedLimiter = new TimeLatch();
            moveRowFocusSpeedLimiter.TimeoutInMilliseconds = 100;
        }
        public override void Dispose() {
            ColumnCreated = null;
            //Removed
            //CustomCreateColumn = null;
            GridDataSourceChanging = null;
            if (popupMenu != null) {
                popupMenu.Dispose();
                popupMenu = null;
            }
            if (gridView != null) {
                UnsubscribeGridViewEvents();
                gridView.CancelNewRow -= new EventHandler(gridView_CancelNewRow);
                gridView.InitNewRow -= new InitNewRowEventHandler(gridView_InitNewRow);
                if (gridView.DataController is XafCurrencyDataController) {
                    ((XafCurrencyDataController)gridView.DataController).NewItemRowObjectCustomAdding -= new HandledEventHandler(gridView_DataController_NewItemRowObjectAdding);
                }
                gridView.Dispose();
                gridView = null;
            }
            if (grid != null) {
                grid.DataSource = null;
                grid.VisibleChanged -= new EventHandler(grid_VisibleChanged);
                grid.KeyDown -= new KeyEventHandler(grid_KeyDown);
                grid.HandleCreated -= new EventHandler(grid_HandleCreated);
                grid.DoubleClick -= new EventHandler(grid_DoubleClick);
                grid.ParentChanged -= new EventHandler(grid_ParentChanged);
                grid.Paint -= new PaintEventHandler(grid_Paint);
                grid.RepositoryItems.Clear();
                grid.Dispose();
                grid = null;
            }
            base.Dispose();
        }
        public override void BreakLinksToControls() {
            base.BreakLinksToControls();
            if (grid != null) {
                grid.Dispose();
                grid = null;
            }
        }
        public string FindColumnPropertyName(GridColumn column) {
            XafLayoutViewColumn layoutViewColumn = column as XafLayoutViewColumn;
            return layoutViewColumn != null ? layoutViewColumn.PropertyName : null;
        }
        protected override ColumnWrapper AddColumnCore(IModelColumn columnInfo) {
            XafLayoutViewColumn column = new XafLayoutViewColumn(ObjectTypeInfo, this);
            LayoutView.Columns.Add(column);
            IMemberInfo memberInfo = FindMemberInfoForColumn(columnInfo);
            if (memberInfo != null) {
                column.FieldName = memberInfo.BindingName;
                if (memberInfo.MemberType.IsEnum) {
                    column.SortMode = ColumnSortMode.Value;
                } else if (!SimpleTypes.IsSimpleType(memberInfo.MemberType)) {
                    column.SortMode = ColumnSortMode.DisplayText;
                }
                if (SimpleTypes.IsClass(memberInfo.MemberType)) {
                    column.FilterMode = ColumnFilterMode.DisplayText;
                } else {
                    column.FilterMode = ColumnFilterMode.Value;
                }
            } else {
                column.FieldName = columnInfo.PropertyName;
            }
            column.ApplyModel(columnInfo);
            if (memberInfo != null) {
                if (repositoryFactory != null) {
                    bool isGranted = DataManipulationRight.CanRead(ObjectType, columnInfo.PropertyName, null, collectionSource);
                    RepositoryItem repositoryItem = null;
                    if (IsReplacedColumnByAsyncServerMode(columnInfo.PropertyName)) {
                        MemberEditorInfoCalculator calculator = new MemberEditorInfoCalculator();
                        Type editorType = calculator.GetEditorType(Model.ModelClass.FindMember(memberInfo.Name));
                        IInplaceEditSupport propertyEditor = Activator.CreateInstance(editorType, ObjectType, columnInfo) as IInplaceEditSupport;
                        repositoryItem = propertyEditor != null ? ((IInplaceEditSupport)propertyEditor).CreateRepositoryItem() : null;
                    } else {
                        repositoryItem = repositoryFactory.CreateRepositoryItem(!isGranted, columnInfo, ObjectType);
                    }
                    if (repositoryItem != null) {
                        grid.RepositoryItems.Add(repositoryItem);
                        repositoryItem.EditValueChanging += new ChangingEventHandler(repositoryItem_EditValueChanging);
                        column.ColumnEdit = repositoryItem;
                        column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : AllowEdit;
                        column.AppearanceCell.Options.UseTextOptions = true;
                        column.AppearanceCell.TextOptions.HAlignment = WinAlignmentProvider.GetAlignment(memberInfo.MemberType);
                        repositoryItem.ReadOnly |= !AllowEdit || ReadOnlyEditors;
                        if (repositoryItem is ILookupEditRepositoryItem) {
                            column.FilterMode = ColumnFilterMode.Value;
                            column.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Equals;
                            column.OptionsFilter.FilterBySortField = DefaultBoolean.False;
                        }
                        if (repositoryItem is RepositoryItemMemoExEdit) {
                            column.OptionsColumn.AllowSort = DefaultBoolean.True;
                        }
                        if ((repositoryItem is RepositoryItemPictureEdit) && (((RepositoryItemPictureEdit)repositoryItem).CustomHeight > 0)) {
                            //Removed
                            //LayoutView.OptionsView.RowAutoHeight = true;
                        }
                        if (repositoryItem is RepositoryItemRtfEditEx) {
                            column.FilterMode = ColumnFilterMode.DisplayText;
                        }
                        if (Model.UseServerMode) {
                            column.FieldNameSortGroup = new ObjectEditorHelperBase(XafTypesInfo.Instance.FindTypeInfo(columnInfo.ModelMember.Type), columnInfo).GetFullDisplayMemberName(columnInfo.PropertyName);
                        }
                        if (!repositoryItem.DisplayFormat.IsEmpty) {
                            column.DisplayFormat.FormatType = repositoryItem.DisplayFormat.FormatType;
                            column.DisplayFormat.FormatString = repositoryItem.DisplayFormat.FormatString;
                        }
                    }
                }
                if ((column.ColumnEdit == null) && !typeof(IList).IsAssignableFrom(memberInfo.MemberType)) {
                    column.OptionsColumn.AllowEdit = false;
                    column.FieldName = GetDisplayablePropertyName(columnInfo.PropertyName);
                }
            }
            OnColumnCreated(column, columnInfo);
            column.LayoutViewField = new LayoutViewField();
            column.LayoutViewField.Name = column.LayoutViewField.ColumnName = column.FieldName;
            if (!LayoutView.TemplateCard.Items.Contains(column.LayoutViewField))
                LayoutView.TemplateCard.Add(column.LayoutViewField);
            if (!grid.IsLoading && gridView.DataController.Columns.GetColumnIndex(column.FieldName) == -1) {
                gridView.DataController.RePopulateColumns();
            }
            return new XafLayoutViewColumnWrapper(column);
        }
        public override void RemoveColumn(ColumnWrapper column) {
            GridColumn gridColumn = ((XafLayoutViewColumnWrapper)column).Column;
            if (LayoutView != null && LayoutView.Columns.Contains(gridColumn)) {
                RemoveColumnInfo(gridColumn);
                LayoutView.Columns.Remove(gridColumn);
            } else {
                throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnDoesNotExist), column.PropertyName), "PropertyName");
            }
        }
        public override void Refresh() {
            if (grid != null) {
                prevFocusedRowHandle = gridView.FocusedRowHandle;
                if (IsAsyncServerMode()) {
                    CollectionSource.ResetCollection();
                }
                grid.RefreshDataSource();
                SelectRowByHandle(prevFocusedRowHandle);
            }
        }
        public override IList GetSelectedObjects() {
            ArrayList selectedObjects = new ArrayList();
            if (LayoutView != null) {
                int[] selectedRows = LayoutView.GetSelectedRows();
                if ((selectedRows != null) && (selectedRows.Length > 0)) {
                    foreach (int rowHandle in selectedRows) {
                        if (!IsGroupRowHandle(rowHandle)) {
                            object obj = MyXtraGridUtils.GetRow(CollectionSource, LayoutView, rowHandle);
                            if (obj != null) {
                                selectedObjects.Add(obj);
                            }
                        }
                    }
                }
            }
            return (object[])selectedObjects.ToArray(typeof(object));
        }
        protected override bool HasProtectedContent(string propertyName) {
            return !(ObjectTypeInfo.FindMember(propertyName) == null || DataManipulationRight.CanRead(ObjectType, propertyName, null, collectionSource));
        }
        public override void StartIncrementalSearch(string searchString) {
            GridColumn defaultColumn = GetDefaultColumn();
            if (defaultColumn != null) {
                LayoutView.FocusedColumn = defaultColumn;
            }
            //Removed
            //LayoutView.StartIncrementalSearch(searchString);
        }
        public IPrintable GetPrintable() {
            return grid;
        }
        public override string[] RequiredProperties {
            get {
                List<string> result = new List<string>();
                if (Model != null) {
                    foreach (IModelColumn columnInfo in Model.Columns) {
                        if ((columnInfo.Index > -1) || (columnInfo.GroupIndex > -1)) {
                            IMemberInfo memberInfo = FindMemberInfoForColumn(columnInfo);
                            if (memberInfo != null) {
                                result.Add(memberInfo.BindingName);
                            }
                        }
                    }
                }
                return result.ToArray();
            }
        }
        public override IContextMenuTemplate ContextMenuTemplate {
            get { return popupMenu; }
        }
        public override string Name {
            get { return base.Name; }
            set {
                base.Name = value;
                SetTag();
            }
        }
        public override object FocusedObject {
            get {
                object result = null;
                if (LayoutView != null) {
                    result = MyXtraGridUtils.GetFocusedRowObject(CollectionSource, LayoutView);
                }
                return result;
            }
            set {
                if (value != null && value != DBNull.Value && gridView != null && DataSource != null) {
                    MyXtraGridUtils.SelectRowByHandle(gridView, gridView.GetRowHandle(List.IndexOf(value)));
                    if (MyXtraGridUtils.HasValidRowHandle(gridView)) {
                        //Removed
                        //gridView.SetRowExpanded(gridView.FocusedRowHandle, true, true);
                        gridView.ExpandCard(gridView.FocusedRowHandle);
                    }
                }
            }
        }
        public override SelectionType SelectionType {
            get { return SelectionType.Full; }
        }
        public RepositoryEditorsFactory RepositoryFactory {
            get { return repositoryFactory; }
            set { repositoryFactory = value; }
        }
        public GridControl Grid {
            get { return grid; }
        }
        public XafLayoutView LayoutView {
            get { return gridView; }
        }
        //Removed
        //public NewItemRowPosition NewItemRowPosition {
        //    get { return newItemRowPosition; }
        //    set {
        //        if(newItemRowPosition != value) {
        //            newItemRowPosition = value;
        //            SetNewItemRow();
        //        }
        //    }
        //}
        //public Boolean ScrollOnMouseMove {
        //    get { return scrollOnMouseMove; }
        //    set { scrollOnMouseMove = value; }
        //}
        public bool ReadOnlyEditors {
            get { return readOnlyEditors; }
            set {
                if (readOnlyEditors != value) {
                    readOnlyEditors = value;
                    AllowEdit = !readOnlyEditors;
                }
            }
        }
        public override IList<ColumnWrapper> Columns {
            get {
                List<ColumnWrapper> result = new List<ColumnWrapper>();
                if (LayoutView != null) {
                    foreach (GridColumn column in LayoutView.Columns) {
                        if (column is XafLayoutViewColumn) {
                            result.Add(new XafLayoutViewColumnWrapper((XafLayoutViewColumn)column));
                        }
                    }
                }
                return result;
            }
        }
        //Removed.
        //public AppearanceFocusedCellMode AppearanceFocusedCellMode {
        //    get {
        //        return appearanceFocusedCellMode;
        //    }
        //    set {
        //        if(appearanceFocusedCellMode != value) {
        //            appearanceFocusedCellMode = value;
        //            UpdateAppearanceFocusedCell();
        //        }
        //    }
        //}
        public override Boolean IsServerModeSupported {
            get { return true; }
        }
        public event EventHandler GridDataSourceChanging;
        public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
        #region IDXPopupMenuHolder Members
        public Control PopupSite {
            get { return Grid; }
        }
        public bool CanShowPopupMenu(Point position) {
            LayoutViewHitTest hitTest = gridView.CalcHitInfo(grid.PointToClient(position)).HitTest;
            return
                 ((hitTest == LayoutViewHitTest.Card)
                 || (hitTest == LayoutViewHitTest.Field)
                 || (hitTest == LayoutViewHitTest.LayoutItem)
                 || (hitTest == LayoutViewHitTest.None));
        }
        public void SetMenuManager(IDXMenuManager manager) {
            if (grid != null) {
                grid.MenuManager = manager;
            }
        }
        #endregion
        #region IControlOrderProvider Members
        public int GetIndexByObject(Object obj) {
            int index = -1;
            if ((DataSource != null) && (gridView != null)) {
                int dataSourceIndex = List.IndexOf(obj);
                index = gridView.GetRowHandle(dataSourceIndex);
                if (index == GridControl.InvalidRowHandle) {
                    index = -1;
                }
            }
            return index;
        }
        public Object GetObjectByIndex(int index) {
            if ((gridView != null) && (gridView.DataController != null)) {
                return gridView.GetRow(index);
            }
            return null;
        }
        public IList GetOrderedObjects() {
            List<Object> list = new List<Object>();
            if (gridView != null && !gridView.IsServerMode) {
                for (int i = 0; i < gridView.DataRowCount; i++) {
                    list.Add(gridView.GetRow(i));
                }
            }
            return list;
        }
        #endregion
        #region IComplexListEditor Members
        public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            this.collectionSource = collectionSource;
            this.application = application;
            repositoryFactory = new RepositoryEditorsFactory(application, collectionSource.ObjectSpace);
        }
        #endregion
        internal CollectionSourceBase CollectionSource {
            get { return collectionSource; }
        }
        #region ILookupListEditor Members
        public Boolean ProcessSelectedItemBySingleClick {
            get { return processSelectedItemBySingleClick; }
            set { processSelectedItemBySingleClick = value; }
        }
        public Boolean TrackMousePosition {
            get { return trackMousePosition; }
            set { trackMousePosition = value; }
        }
        public event EventHandler BeginCustomization;
        public event EventHandler EndCustomization;
        #endregion
        #region ISupportConditionalFormatting Members
        public event EventHandler<ObjectConditionalFormattingEventArgs> ObjectConditionalFormatting;
        #endregion
        #region ISupportAppearanceCustomization Members
        public event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
        #endregion
        #region IHtmlFormattingSupport Members
        private bool htmlFormattingEnabled;
        public void SetHtmlFormattingEnabled(bool htmlFormattingEnabled) {
            this.htmlFormattingEnabled = htmlFormattingEnabled;
            if (this.LayoutView != null) {
                ApplyHtmlFormatting();
            }
        }
        private void ApplyHtmlFormatting() {
            //Removed
            //this.gridView.OptionsView.AllowHtmlDrawHeaders = htmlFormattingEnabled;
            gridView.Appearance.HeaderPanel.TextOptions.WordWrap = htmlFormattingEnabled ? WordWrap.Wrap : WordWrap.Default;
        }
        #endregion
        #region IFocusedElementCaptionProvider Members
        object IFocusedElementCaptionProvider.FocusedElementCaption {
            get {
                if (LayoutView != null) {
                    return LayoutView.GetFocusedDisplayText();
                }
                return null;
            }
        }
        #endregion
        //Removed
        //#region ISummaryFooter Members
        //bool ISupportFooter.IsFooterVisible {
        //    get {
        //        return LayoutView.OptionsView.ShowFooter;
        //    }
        //    set {
        //        LayoutView.OptionsView.ShowFooter = value;
        //    }
        //}
        //#endregion
        #region IExportableEditor Members
        public IList<PrintingSystemCommand> ExportTypes {
            get {
                IList<PrintingSystemCommand> exportTypes = new List<PrintingSystemCommand>();
                exportTypes.Add(PrintingSystemCommand.ExportXls);
                exportTypes.Add(PrintingSystemCommand.ExportHtm);
                exportTypes.Add(PrintingSystemCommand.ExportTxt);
                exportTypes.Add(PrintingSystemCommand.ExportMht);
                exportTypes.Add(PrintingSystemCommand.ExportPdf);
                exportTypes.Add(PrintingSystemCommand.ExportRtf);
                exportTypes.Add(PrintingSystemCommand.ExportGraphic);
                return exportTypes;
            }
        }
        public IPrintable Printable {
            get { return printable; }
            set {
                if (printable != value) {
                    printable = value;
                    OnPrintableChanged();
                }
            }
        }
        public event EventHandler<PrintableChangedEventArgs> PrintableChanged;
        #endregion
        #region ILookupEditProvider Members
        private event EventHandler<LookupEditProviderEventArgs> lookupEditCreated;
        private event EventHandler<LookupEditProviderEventArgs> lookupEditHide;
        protected void OnLookupEditCreated(LookupEdit control) {
            if (lookupEditCreated != null) {
                lookupEditCreated(this, new LookupEditProviderEventArgs(control));
            }
        }
        protected void OnLookupEditHide(LookupEdit control) {
            if (lookupEditHide != null) {
                lookupEditHide(this, new LookupEditProviderEventArgs(control));
            }
        }
        event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
            add { lookupEditCreated += value; }
            remove { lookupEditCreated -= value; }
        }
        event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
            add { lookupEditHide += value; }
            remove { lookupEditHide -= value; }
        }
        #endregion
    }
    public class MyXtraGridUtils {
        public static bool HasValidRowHandle(LayoutView view) {
            return ((view.GridControl.DataSource != null) && (view.FocusedRowHandle >= 0) && (view.RowCount > 0));
        }
        public static void SelectFocusedRow(LayoutView view) {
            SelectRowByHandle(view, view.FocusedRowHandle);
        }
        public static void SelectRowByHandle(LayoutView view, int rowHandle) {
            if (rowHandle != GridControl.InvalidRowHandle && view.GridControl != null) {
                view.BeginSelection();
                try {
                    view.ClearSelection();
                    view.SelectRow(rowHandle);
                    view.FocusedRowHandle = rowHandle;
                } finally {
                    view.EndSelection();
                }
            }
        }
        public static object GetFocusedRowObject(LayoutView view) {
            return GetRow(view, view.FocusedRowHandle);
        }
        public static object GetNearestRowObject(LayoutView view) {
            object result = GetRow(view, view.FocusedRowHandle + 1);
            if (result == null) {
                result = GetRow(view, view.FocusedRowHandle - 1);
            }
            return result;
        }
        public static object GetRow(LayoutView view, int rowHandle) {
            return GetRow(null, view, rowHandle);
        }
        public static bool IsRowSelected(LayoutView view, int rowHandle) {
            int[] selected = view.GetSelectedRows();
            for (int i = 0; (selected != null) && (i < selected.Length - 1); i++) {
                if (selected[i] == rowHandle) {
                    return true;
                }
            }
            return false;
        }
        public static Object GetRow(CollectionSourceBase collectionSource, LayoutView view, int rowHandle) {
            if (
                (!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))
                ||
                (view.GridControl.DataSource == null)
                ||
                ((view.DataSource != view.GridControl.DataSource) && !view.IsServerMode)) {
                return null;
            }
            if ((collectionSource is CollectionSource) && ((CollectionSource)collectionSource).IsServerMode && ((CollectionSource)collectionSource).IsAsyncServerMode) {
                if (!view.IsRowLoaded(rowHandle)) {
                    return null;
                }
                String keyPropertyName = "";
                if (collectionSource.ObjectTypeInfo.KeyMember != null) {
                    keyPropertyName = collectionSource.ObjectTypeInfo.KeyMember.Name;
                }
                if (!String.IsNullOrEmpty(keyPropertyName)) {
                    Object objectKey = view.GetRowCellValue(rowHandle, keyPropertyName);
                    return collectionSource.ObjectSpace.GetObjectByKey(collectionSource.ObjectTypeInfo.Type, objectKey);
                }
            }
            object result = view.GetRow(rowHandle);
            if (result == null && view is XafLayoutView && ((XafLayoutView)view).IsNewItemRowCancelling) {
                result = ((XafLayoutView)view).NewItemRowObject;
            }
            return result;
        }
        public static Object GetFocusedRowObject(CollectionSourceBase collectionSource, LayoutView view) {
            return GetRow(collectionSource, view, view.FocusedRowHandle);
        }
    }
    internal class CancelEventArgsAppearanceAdapter : IAppearanceEnabled, IAppearanceItem {
        private CancelEventArgs cancelEdit;
        public CancelEventArgsAppearanceAdapter(CancelEventArgs cancelEdit) {
            this.cancelEdit = cancelEdit;
        }
        #region IAppearanceEnabled Members
        public bool Enabled {
            get { return !cancelEdit.Cancel; }
            set { cancelEdit.Cancel = !value; }
        }
        #endregion
        #region IAppearanceItem Members
        public object Data {
            get { return cancelEdit; }
        }
        #endregion
    }
    internal class AppearanceObjectAdapterWithReset : AppearanceObjectAdapter, IAppearanceReset {
        private AppearanceObject appearanceObject;
        public AppearanceObjectAdapterWithReset(AppearanceObject appearanceObject, object data)
            : base(appearanceObject, data) {
            this.appearanceObject = appearanceObject;
        }
        public void ResetAppearance() {
            appearanceObject.Reset();
        }
    }
}