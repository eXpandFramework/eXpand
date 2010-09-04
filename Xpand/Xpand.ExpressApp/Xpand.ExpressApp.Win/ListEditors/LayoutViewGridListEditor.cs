
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.NodeWrappers;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Localization;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.SystemModule;

namespace eXpand.ExpressApp.Win.ListEditors{
    public class XafLayoutView : LayoutView {
        private ErrorMessages errorMessages;
        private object GetFocusedObject() {
            return this.GetRow(this.FocusedRowHandle);
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
        protected override void RaiseInvalidRowException(InvalidRowExceptionEventArgs ex) {
            ex.ExceptionMode = ExceptionMode.ThrowException;
            base.RaiseInvalidRowException(ex);
        }
        protected override void OnActiveEditor_MouseDown(object sender, MouseEventArgs e) {
            if (ActiveEditor != null) {
                base.OnActiveEditor_MouseDown(sender, e);
            }
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
        public ErrorMessages ErrorMessages {
            get { return errorMessages; }
            set { errorMessages = value; }
        }
        public event EventHandler FilterEditorPopup;
        public event EventHandler FilterEditorClosed;
    }

    public class ColumnCreatedEventArgs : EventArgs {
        private LayoutViewColumn column;
        private IModelColumn columnInfo;
        public ColumnCreatedEventArgs(LayoutViewColumn column, IModelColumn columnInfo) {
            this.column = column;
            this.columnInfo = columnInfo;
        }
        public LayoutViewColumn Column {
            get { return column; }
            set { column = value; }
        }
        public IModelColumn ColumnInfo {
            get { return columnInfo; }
            set { columnInfo = value; }
        }
    }

    public class LayoutViewGridListEditor : ListEditor, IControlOrderProvider, IDXPopupMenuHolder, IComplexListEditor, IPrintableSource, ILookupListEditor {
        public const string IsGroupPanelVisible = "IsGroupPanelVisible";
        public const string ActiveFilterString = "ActiveFilterString";
        public const string IsFooterVisible = "IsFooterVisible";
        public const string IsActiveFilterEnabled = "IsActiveFilterEnabled";
        public const string DragEnterCustomCodeId = "DragEnter";
        public const string DragDropCustomCodeId = "DragDrop";
        public const int ColumnDefaultWidth = 50;
        private RepositoryEditorsFactory repositoryFactory;
        private EditMode editMode = EditMode.Editable;
        private GridControl grid;
        private XafLayoutView layoutView;
        private int mouseDownTime;
        private int mouseUpTime;
        private bool activatedByMouse = false;
        private bool focusedChangedRaised;
        private bool selectedChangedRaised;
        private bool isForceSelectRow;
        private int visibleIndexOnChanged;
        private RepositoryItem activeEditor;
        private Dictionary<LayoutViewColumn, string> columnsProperties = new Dictionary<LayoutViewColumn, string>();
        private ActionsDXPopupMenu popupMenu;
        private Boolean processSelectedItemBySingleClick;
        private TimeAutoLatch moveRowFocusSpeedLimiter = new TimeAutoLatch();
        private bool selectedItemActionExecuting = false;
        private XafLayoutView CreateLayotView() {
            layoutView = new XafLayoutView();
            layoutView.TemplateCard = new LayoutViewCard();
            layoutView.ErrorMessages = ErrorMessages;
            layoutView.ShowingEditor += new CancelEventHandler(LayoutView_EditorShowing);
            layoutView.ShownEditor += new EventHandler(LayoutView_ShownEditor);
            layoutView.HiddenEditor += new EventHandler(LayoutView_HiddenEditor);
            layoutView.MouseDown += new MouseEventHandler(LayoutView_MouseDown);
            layoutView.MouseUp += new MouseEventHandler(LayoutView_MouseUp);
            layoutView.FocusedRowChanged += new FocusedRowChangedEventHandler(LayoutView_FocusedRowChanged);
            layoutView.SelectionChanged += new SelectionChangedEventHandler(LayoutView_SelectionChanged);
            layoutView.Click += new EventHandler(LayoutView_Click);
            layoutView.MouseWheel += new MouseEventHandler(LayoutView_MouseWheel);
            if (editMode == EditMode.Editable) {
                layoutView.ValidateRow += new ValidateRowEventHandler(LayoutView_ValidateRow);
                layoutView.InitNewRow += new InitNewRowEventHandler(LayoutView_InitNewRow);
            }
            layoutView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
            layoutView.OptionsBehavior.Editable = true;
            layoutView.OptionsBehavior.AutoSelectAllInEditor = false;
            layoutView.OptionsBehavior.AutoPopulateColumns = false;
            layoutView.OptionsBehavior.FocusLeaveOnTab = true;
            layoutView.OptionsSelection.MultiSelect = true;
            layoutView.ShowButtonMode = ShowButtonModeEnum.ShowOnlyInEditor;
            layoutView.ActiveFilterEnabled = ((IModelListViewWin)Model).IsActiveFilterEnabled;
            return layoutView;
        }
        private void LayoutView_InitNewRow(object sender, InitNewRowEventArgs e) {
            OnNewObjectCreated();
        }
        private void LayoutView_MouseWheel(object sender, MouseEventArgs e) {
            moveRowFocusSpeedLimiter.Reset();
        }
        private void LayoutView_HideCustomizationForm(object sender, EventArgs e) {
            if (EndCustomization != null)
                EndCustomization(this, EventArgs.Empty);
        }
        private void LayoutView_ShowCustomizationForm(object sender, EventArgs e) {
            if (BeginCustomization != null)
                BeginCustomization(this, EventArgs.Empty);
        }
        private bool IsGroupRowHandle(int handle) {
            return handle < 0;
        }
        private void grid_HandleCreated(object sender, EventArgs e) {
            AssignDataSourceToControl(this.DataSource);
        }
        private void grid_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
            if (FocusedObject != null && e.KeyCode == Keys.Enter) {
                if ((editMode != EditMode.ReadOnlyEditors || (LayoutView.ActiveEditor == null))) {
                    this.OnProcessSelectedItem();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                } else {
                    if ((editMode != EditMode.ReadOnly) && (LayoutView.ActiveEditor == null)) {
                        if (layoutView.IsLastColumnFocused) {
                            layoutView.UpdateCurrentRow();
                            e.Handled = true;
                        } else {
                            LayoutView.FocusedColumn =
                                LayoutView.GetVisibleColumn(1 + layoutView.VisibleColumns.IndexOf(LayoutView.FocusedColumn));
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
        private void SubmitActiveEditorChanges() {
            if ((LayoutView.ActiveEditor != null) && LayoutView.ActiveEditor.IsModified) {
                LayoutView.PostEditor();
                LayoutView.UpdateCurrentRow();
            }
        }
        private void grid_DoubleClick(object sender, EventArgs e) {
            ProcessMouseClick(e);
        }
        private void LayoutView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            isForceSelectRow = e.Action == CollectionChangeAction.Add;
            OnSelectionChanged();
        }
        private void LayoutView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
            if (DataSource != null && !(DataSource is XPBaseCollection)) {
                visibleIndexOnChanged = e.PrevFocusedRowHandle;
            }
            OnFocusedObjectChanged();
        }
        private void LayoutView_Click(object sender, EventArgs e) {
            if (processSelectedItemBySingleClick) {
                ProcessMouseClick(e);
            }
        }
        private void LayoutView_ValidateRow(object sender, ValidateRowEventArgs e) {
        }
        private void LayoutView_EditorShowing(object sender, CancelEventArgs e) {
            activeEditor = null;
            RepositoryItem edit = layoutView.FocusedColumn.ColumnEdit;
            if (edit != null) {
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
        private void LayoutView_ShownEditor(object sender, EventArgs e) {
            PopupBaseEdit popupEdit = layoutView.ActiveEditor as PopupBaseEdit;
            if (popupEdit != null && activatedByMouse) {
                popupEdit.ShowPopup();
            }
            activatedByMouse = false;
        }
        private void LayoutView_HiddenEditor(object sender, EventArgs e) {
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
        private void LayoutView_MouseDown(object sender, MouseEventArgs e) {
            LayoutView view = (LayoutView)sender;
            LayoutViewHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
            if (hi.RowHandle >= 0) {
                mouseDownTime = System.Environment.TickCount;
            } else {
                mouseDownTime = 0;
            }
            activatedByMouse = true;
        }
        private void LayoutView_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = System.Environment.TickCount;
            LayoutViewHitInfo hi = ((LayoutView)sender).CalcHitInfo(new Point(e.X, e.Y));
            if (hi.RowHandle == BaseListSourceDataController.NewItemRow) {
                layoutView.ShowEditorByMouse();
            }
        }
        private void LayoutView_CustomDrawFooterCell(object sender, FooterCellCustomDrawEventArgs e) {
            if (e.Info.Visible) {
                switch (e.Column.SummaryItem.SummaryType) {
                    case SummaryItemType.Sum:
                    case SummaryItemType.Average:
                    case SummaryItemType.Max:
                    case SummaryItemType.Min:
                        e.Info.DisplayText = string.Format("{0}={1}", e.Column.SummaryItem.SummaryType.ToString(), (e.Column.ColumnEdit != null) ? e.Column.ColumnEdit.DisplayFormat.GetDisplayText(e.Info.Value) : e.Info.Value.ToString());
                        break;
                }
            }
        }
        private void Editor_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Int32 currentTime = System.Environment.TickCount;
                if ((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < SystemInformation.DoubleClickTime)) {
                    this.OnProcessSelectedItem();
                    mouseDownTime = 0;
                }
            }
        }
        private void Editor_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = System.Environment.TickCount;
        }
        private void Editor_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                SubmitActiveEditorChanges();
            }
        }
        private void SpinEdit_Spin(object sender, SpinEventArgs e) {
            mouseDownTime = 0;
        }
        private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e) {
            mouseDownTime = 0;
        }
        private void DataSource_ListChanged(object sender, ListChangedEventArgs e) {
            if ((grid != null) && (grid.FindForm() != null) && !grid.ContainsFocus) {
                if ((e.ListChangedType == ListChangedType.ItemAdded) && (((IList)sender).Count == 1)) {
                    IEditableObject obj = ((IList)sender)[e.NewIndex] as IEditableObject;
                    if (obj != null) {
                        obj.EndEdit();
                    }
                }
            }
            if (e.ListChangedType == ListChangedType.ItemChanged) {
                visibleIndexOnChanged = layoutView.GetVisibleIndex(layoutView.FocusedRowHandle);
            }
            if (e.ListChangedType == ListChangedType.ItemDeleted && layoutView.FocusedRowHandle != BaseListSourceDataController.NewItemRow) {
                layoutView.FocusedRowHandle = layoutView.GetVisibleRowHandle(visibleIndexOnChanged);
                OnFocusedObjectChanged();
            }
            if (layoutView != null) {
                if (e.ListChangedType == ListChangedType.Reset && layoutView.SelectedRowsCount == 0) {
                    layoutView.SelectRow(layoutView.FocusedRowHandle);
                }
            }
        }
        private void SetTag() {
            if (grid != null) {
                grid.Tag = EasyTestTagHelper.FormatTestTable(Name);
            }
        }
        private void RefreshColumn(IModelColumn frameColumn, LayoutViewColumn column) {
            column.Caption = frameColumn.Caption;
            if (string.IsNullOrEmpty(column.Caption)) {
                column.Caption = column.FieldName;
            }
            column.LayoutViewField.ColumnName = column.Caption;
            if (!string.IsNullOrEmpty(frameColumn.DisplayFormat)) {
                column.DisplayFormat.FormatString = frameColumn.DisplayFormat;
                column.DisplayFormat.FormatType = FormatType.Custom;
                column.GroupFormat.FormatString = frameColumn.DisplayFormat;
                column.GroupFormat.FormatType = FormatType.Custom;
            }
            column.GroupIndex = frameColumn.GroupIndex;
            column.SortIndex = frameColumn.SortIndex;
            column.SortOrder = frameColumn.SortOrder;
            column.Width = frameColumn.Width;
            if (column.VisibleIndex != frameColumn.Index) {
                column.VisibleIndex = frameColumn.Index;
            }
            
            column.SummaryItem.SummaryType = (SummaryItemType)Enum.Parse(typeof(SummaryItemType), frameColumn.SummaryType.ToString());
        }
        protected virtual void ProcessMouseClick(EventArgs e) {
            if (!selectedItemActionExecuting) {
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

        protected virtual void OnColumnCreated(LayoutViewColumn column, IModelColumn columnInfo) {
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
                LayoutView.SelectRow(LayoutView.FocusedRowHandle);
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
                    grid.VisibleChanged += new EventHandler(grid_VisibleChanged);
                    grid.Height = 100;
                    grid.TabStop = true;
                    grid.MainView = CreateLayotView();
                    SetTag();
                    LayoutView.Columns.Clear();
                    RefreshColumns();
                } finally {
                    ((System.ComponentModel.ISupportInitialize)(grid)).EndInit();
                    layoutView.ForceLoaded();
                }
            }
            return grid;
        }
        private void grid_VisibleChanged(object sender, EventArgs e) {
            if (grid.Visible) {
                grid.VisibleChanged -= new EventHandler(grid_VisibleChanged);
                LayoutViewColumn defaultColumn = GetDefaultColumn();
                if (defaultColumn != null)
                    layoutView.FocusedColumn = defaultColumn;
            }
        }
        private void grid_ParentChanged(object sender, EventArgs e) {
            if (grid.Parent != null) {
                layoutView.ForceLoaded();
            }
        }
        private LayoutViewColumn GetDefaultColumn() {
            LayoutViewColumn result = null;

            IMemberInfo defaultMember = Model.ModelClass.TypeInfo.DefaultMember;
                if (defaultMember != null) {
                    result = LayoutView.Columns[defaultMember.Name];
                }

            return result == null || !result.Visible ? null : result;
        }
        private void RemoveColumnInfo(LayoutViewColumn column) {
            String originalPropertyName = columnsProperties[column];
            IModelColumn columnInfo = Model.Columns[originalPropertyName];
            if (columnInfo != null) {
                Model.Columns.Remove(columnInfo);
            }
        }
        protected override void OnProcessSelectedItem() {
            if ((layoutView != null) && (layoutView.ActiveEditor != null)) {
                BindingHelper.EndCurrentEdit(Grid);
            }

            base.OnProcessSelectedItem();
        }

        protected internal bool IsDataShownOnDropDownWindow(RepositoryItem repositoryItem) {
            return DXPropertyEditor.RepositoryItemsTypesWithMandatoryButtons.Contains(repositoryItem.GetType());
        }
        protected override void AssignDataSourceToControl(object o) {
            if (grid != null && grid.DataSource != DataSource) {
                if (grid.DataSource is IBindingList) {
                    ((IBindingList)grid.DataSource).ListChanged -= new ListChangedEventHandler(DataSource_ListChanged);
                }
                if (grid.IsHandleCreated) {
                    focusedChangedRaised = false;
                    selectedChangedRaised = false;
                    OnGridDataSourceChanging();
                    grid.BeginUpdate();
                    try {
                        grid.DataSource = DataSource;
                    } finally {
                        grid.EndUpdate();
                    }
                    if (!selectedChangedRaised) {
                        OnSelectionChanged();
                    }
                    if (!focusedChangedRaised) {
                        OnFocusedObjectChanged();
                    }
                    if (grid.DataSource is IBindingList) {
                        ((IBindingList)grid.DataSource).ListChanged += DataSource_ListChanged;
                    }
                }
            }
        }
        public LayoutViewGridListEditor(IModelListView model)
            : base(model) {
            popupMenu = new ActionsDXPopupMenu();
        }
        public LayoutViewGridListEditor() : base() { }
        public LayoutViewColumn AddColumn(IModelColumn columnInfo) {
            if (columnsProperties.ContainsValue(columnInfo.PropertyName)) {
                throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnExists), columnInfo.PropertyName), "ColumnInfo");
            }
            
            LayoutViewColumn column = new LayoutViewColumn();
            columnsProperties.Add(column, columnInfo.PropertyName);
            LayoutView.Columns.Add(column);
            
                IMemberInfo memberDescriptor = XafTypesInfo.Instance.FindTypeInfo(ObjectType).FindMember(columnInfo.PropertyName);
                if (memberDescriptor != null) {
                    column.FieldName = memberDescriptor.BindingName;
                    if (memberDescriptor.MemberType.IsEnum) {
                        column.SortMode = ColumnSortMode.Value;
                    } else if (!SimpleTypes.IsSimpleType(memberDescriptor.MemberType)) {
                        column.SortMode = ColumnSortMode.DisplayText;
                    }
                    if (SimpleTypes.IsClass(memberDescriptor.MemberType)) {
                        column.FilterMode = ColumnFilterMode.DisplayText;
                    } else {
                        column.FilterMode = ColumnFilterMode.Value;
                    }
                } else {
                    column.FieldName = columnInfo.PropertyName;
                }
                RefreshColumn(columnInfo, column);
                if (memberDescriptor != null) {
                    if (repositoryFactory != null) {
                        RepositoryItem repositoryItem = repositoryFactory.CreateRepositoryItem(false, columnInfo, ObjectType);
                        if (repositoryItem != null) {
                            grid.RepositoryItems.Add(repositoryItem);
                            column.ColumnEdit = repositoryItem;
                            column.OptionsColumn.AllowEdit = IsDataShownOnDropDownWindow(repositoryItem) ? true : editMode != EditMode.ReadOnly;
                            repositoryItem.ReadOnly |= editMode != EditMode.Editable;
                            if ((repositoryItem is ILookupEditRepositoryItem) && ((ILookupEditRepositoryItem)repositoryItem).IsFilterByValueSupported) {
                                column.FilterMode = ColumnFilterMode.Value;
                            }
                        }
                    }
                    if ((column.ColumnEdit == null) && !typeof(IList).IsAssignableFrom(memberDescriptor.MemberType)) {
                        column.OptionsColumn.AllowEdit = false;
                        column.FieldName = GetDisplayablePropertyName(columnInfo.PropertyName);
                    }
                }
            
            OnColumnCreated(column, columnInfo);
            
            return column;
        }
        public void RemoveColumn(string propertyName) {
            bool found = false;
            if (LayoutView != null) {
                foreach (LayoutViewColumn column in LayoutView.Columns) {
                    if ((column.FieldName == propertyName) || (column.FieldName == propertyName + "!")) {
                        RemoveColumnInfo(column);
                        columnsProperties.Remove(column);
                        LayoutView.Columns.Remove(column);
                        found = true;
                        break;
                    }
                }
            }
            if (!found) {
                throw new ArgumentException(string.Format(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.GridColumnDoesNotExist), propertyName), "PropertyName");
            }
        }
        public void RefreshColumns() {
            Grid.BeginUpdate();
            try {
                Dictionary<string, LayoutViewColumn> presentedColumns = new Dictionary<string, LayoutViewColumn>();
                List<LayoutViewColumn> toDelete = new List<LayoutViewColumn>();
                foreach (LayoutViewColumn column in LayoutView.Columns) {
                    presentedColumns.Add(columnsProperties[column], column);
                    toDelete.Add(column);
                }
                foreach (IModelColumn column in Model.Columns) {
                    LayoutViewColumn LayoutViewColumn = null;
                    if (presentedColumns.TryGetValue(column.PropertyName, out LayoutViewColumn)) {
                        RefreshColumn(column, LayoutViewColumn);
                    } else {
                        LayoutViewColumn = AddColumn(column);
                        presentedColumns.Add(column.PropertyName, LayoutViewColumn);
                    }
                    toDelete.Remove(LayoutViewColumn);
                }
                foreach (LayoutViewColumn LayoutViewColumn in toDelete) {
                    LayoutView.Columns.Remove(LayoutViewColumn);
                    columnsProperties.Remove(LayoutViewColumn);
                }
            } finally {
                Grid.EndUpdate();
            }
        }
        public override void Refresh() {
            if (grid != null) {
                grid.RefreshDataSource();
            }
        }
        public override void Dispose() {
            ColumnCreated = null;
            CustomCreateColumn = null;
            GridDataSourceChanging = null;
            if (popupMenu != null) {
                popupMenu.Dispose();
                popupMenu = null;
            }
            columnsProperties.Clear();
            if (layoutView != null) {
                layoutView.FocusedRowChanged -= new FocusedRowChangedEventHandler(LayoutView_FocusedRowChanged);
                layoutView.SelectionChanged -= new SelectionChangedEventHandler(LayoutView_SelectionChanged);
                layoutView.ShowingEditor -= new CancelEventHandler(LayoutView_EditorShowing);
                layoutView.ShownEditor -= new EventHandler(LayoutView_ShownEditor);
                layoutView.HiddenEditor -= new EventHandler(LayoutView_HiddenEditor);
                layoutView.MouseDown -= new MouseEventHandler(LayoutView_MouseDown);
                layoutView.MouseUp -= new MouseEventHandler(LayoutView_MouseUp);
                layoutView.Click -= new EventHandler(LayoutView_Click);
                layoutView.ValidateRow -= new ValidateRowEventHandler(LayoutView_ValidateRow);
                layoutView.InitNewRow -= new InitNewRowEventHandler(LayoutView_InitNewRow);
                layoutView.Dispose();
                layoutView = null;
            }
            if (grid != null) {
                if (grid.DataSource is IBindingList) {
                    ((IBindingList)grid.DataSource).ListChanged -= new ListChangedEventHandler(DataSource_ListChanged);
                }
                grid.DataSource = null;
                grid.VisibleChanged -= new EventHandler(grid_VisibleChanged);
                grid.KeyDown -= new KeyEventHandler(grid_KeyDown);
                grid.HandleCreated -= new EventHandler(grid_HandleCreated);
                grid.DoubleClick -= new EventHandler(grid_DoubleClick);
                grid.ParentChanged -= new EventHandler(grid_ParentChanged);
                grid.RepositoryItems.Clear();
                grid.Dispose();
                grid = null;
            }
            base.Dispose();
        }
        public override IList GetSelectedObjects() {
            ArrayList selectedObjects = new ArrayList();
            if (LayoutView != null) {
                int[] selectedRows = LayoutView.GetSelectedRows();
                if ((selectedRows != null) && (selectedRows.Length > 0)) {
                    foreach (int rowHandle in selectedRows) {
                        if (!IsGroupRowHandle(rowHandle)) {
                            object obj = LayoutView.GetRow(rowHandle);
                            if (obj != null) {
                                selectedObjects.Add(obj);
                            }
                        }
                    }
                }
            }
            return (object[])selectedObjects.ToArray(typeof(object));
        }
        public override void SynchronizeInfo() {
            if (LayoutView != null) {
                Model.IsGroupPanelVisible = layoutView.OptionsView.ShowHeaderPanel;
                Model.IsFooterVisible = layoutView.OptionsView.ShowCardLines;
                foreach (LayoutViewColumn column in LayoutView.Columns) {
                    string propertyName;
                    if (columnsProperties.TryGetValue(column, out propertyName)) {
                        IModelColumn frameColumn = Model.Columns[propertyName];
                        if (column.Caption != frameColumn.Caption) {
                            frameColumn.Caption = column.Caption;
                        }
                        if (column.Width != frameColumn.Width) {
                            frameColumn.Width = column.Width;
                        }
                        if (column.GroupIndex != frameColumn.GroupIndex) {
                            frameColumn.GroupIndex = column.GroupIndex;
                        }
                        if (frameColumn.SortIndex != column.SortIndex) {
                            frameColumn.SortIndex = column.SortIndex;
                        }
                        if (frameColumn.SortOrder != column.SortOrder) {
                            frameColumn.SortOrder = column.SortOrder;
                        }
                        if (frameColumn.Index != column.VisibleIndex) {
                            frameColumn.Index = column.VisibleIndex;
                        }
                        var summaryType = (SummaryType)Enum.Parse(typeof(SummaryType), column.SummaryItem.SummaryType.ToString());
                        if (frameColumn.SummaryType != summaryType) {
                            frameColumn.SummaryType = summaryType;
                        }
                    }
                }
            }
        }
        public override void StartIncrementalSearch(string searchString) {
            LayoutViewColumn defaultColumn = GetDefaultColumn();
            if (defaultColumn != null) {
                LayoutView.FocusedColumn = defaultColumn;
            }
        }
        public IPrintable GetPrintable() {
            return grid;
        }
        private String ReplaceExclamationMarks(String memberName) {
            return memberName.TrimEnd('!').Replace('!', '.');
        }
        public override string[] ShownProperties {
            get {
                List<string> result = new List<string>();
                if ((ObjectType != null) && (LayoutView != null)) {
                    foreach (LayoutViewColumn column in LayoutView.VisibleColumns) {
                        if (column.ColumnEdit is IShownPropertiesProvider) {
                            string[] editorShownProperties = ((IShownPropertiesProvider)column.ColumnEdit).ShownProperties;
                            foreach (string propertyName in editorShownProperties) {
                                if (!string.IsNullOrEmpty(propertyName)) {
                                    result.Add(ReplaceExclamationMarks(column.FieldName) + '.' + propertyName);
                                } else {
                                    result.Add(ReplaceExclamationMarks(column.FieldName));
                                }
                            }
                        } else {
                            result.Add(ReplaceExclamationMarks(column.FieldName));
                        }
                    }
                }
                return result.ToArray();
            }
        }
        public override string[] RequiredProperties {
            get {
                List<string> result = new List<string>();
                if ((ObjectType != null) && (LayoutView != null)) {
                    foreach (LayoutViewColumn column in LayoutView.VisibleColumns) {
                        result.Add(column.FieldName);
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
        public override EditMode EditMode {
            get { return editMode; }
            set {
                if (grid != null)
                    throw new InvalidOperationException("Cannot set EditMode property. GridControl have been created already.");
                editMode = value;
            }
        }
        public override object FocusedObject {
            get {
                object result = null;
                if (LayoutView != null) {
                    result = LayoutView.GetRow(LayoutView.FocusedRowHandle);
                }
                return result;
            }
            set {
                if ((value != null) && (layoutView != null) && (DataSource != null)) {
                    layoutView.SelectRow(layoutView.GetRowHandle(List.IndexOf(value)));
                    if (layoutView.IsValidRowHandle(layoutView.FocusedRowHandle)) {
                        layoutView.ExpandCard(layoutView.FocusedRowHandle);
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
            get { return layoutView; }
        }
        #region IDXPopupMenuHolder Members
        public Control PopupSite {
            get { return Grid; }
        }
        public bool CanShowPopupMenu(Point position) {
            LayoutViewHitTest hitTest = layoutView.CalcHitInfo(grid.PointToClient(position)).HitTest;
            return
                ((hitTest == LayoutViewHitTest.Card)
                 || (hitTest == LayoutViewHitTest.Field)
                 || (hitTest == LayoutViewHitTest.LayoutItem)
                 || (hitTest == LayoutViewHitTest.None));
        }
        public void SetMenuManager(IDXMenuManager manager) {
            grid.MenuManager = manager;
        }
        #endregion
        #region IControlOrderProvider Members
        public int GetIndexByObject(Object obj) {
            int index = -1;
            if ((DataSource != null) && (layoutView != null)) {
                int dataSourceIndex = List.IndexOf(obj);
                index = layoutView.GetRowHandle(dataSourceIndex);
                if (index == GridControl.InvalidRowHandle) {
                    index = -1;
                }
            }
            return index;
        }
        public Object GetObjectByIndex(int index) {
            if ((layoutView != null) && (layoutView.DataController != null)) {
                return layoutView.GetRow(index);
            }
            return null;
        }
        public IList GetOrderedObjects() {
            List<Object> list = new List<Object>();
            if (layoutView != null && layoutView.GridControl != null && !grid.ServerMode) {
                for (int i = 0; i < layoutView.DataRowCount; i++) {
                    list.Add(layoutView.GetRow(i));
                }
            }
            return list;
        }
        #endregion
        #region IComplexListEditor Members
        public virtual void Setup(CollectionSourceBase collectionSource, XafApplication application) {
            repositoryFactory = new RepositoryEditorsFactory(application, collectionSource.ObjectSpace);
        }
        #endregion
        #region ILookupListEditor Members
        public Boolean ProcessSelectedItemBySingleClick {
            get { return processSelectedItemBySingleClick; }
            set { processSelectedItemBySingleClick = value; }
        }
        public event EventHandler BeginCustomization;
        public event EventHandler EndCustomization;
        #endregion
        public event EventHandler<ColumnCreatedEventArgs> ColumnCreated;
        public event EventHandler<CustomCreateColumnEventArgs> CustomCreateColumn;
        public event EventHandler GridDataSourceChanging;

        #region ILookupListEditor Member


        public bool TrackMousePosition {
            get {
                return false;
            }
            set {
                return;
            }
        }

        #endregion
    }

    internal class TimeAutoLatch {
        private long lastEventTicks;
        private int timeIntervalInMs;
        public TimeAutoLatch(int timeIntervalInMs) {
            this.timeIntervalInMs = timeIntervalInMs;
            this.lastEventTicks = 0;
        }
        public TimeAutoLatch() : this(100) { }
        public bool IsTimeIntervalExpired {
            get {
                bool result = ((DateTime.Now.Ticks - lastEventTicks) / 10000) > timeIntervalInMs;
                if (result) {
                    lastEventTicks = DateTime.Now.Ticks;
                }
                return result;
            }
        }
        public void Reset() {
            lastEventTicks = 0;
        }
    }
}