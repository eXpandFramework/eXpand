using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Filter;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Views.Layout.Customization;
using DevExpress.XtraGrid.Views.Layout.ViewInfo;
using DevExpress.XtraLayout;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView {
    public class XafLayoutView : DevExpress.XtraGrid.Views.Layout.LayoutView, IModelSynchronizersHolder {
        public Dictionary<Component, IModelSynchronizer> ColumnsInfoCache = new Dictionary<Component, IModelSynchronizer>();
        protected override void AssignColumns(DevExpress.XtraGrid.Views.Base.ColumnView cv, bool synchronize) {
            base.AssignColumns(cv, synchronize);
            if (!synchronize) {
                ((IModelSynchronizersHolder)this).AssignSynchronizers(cv);
            }
        }

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
            IModelSynchronizersHolder current = this;
            IModelSynchronizersHolder sourceInfoProvider = (IModelSynchronizersHolder)sourceView;
            for (int n = 0; n < sourceView.Columns.Count; n++) {
                IGridColumnModelSynchronizer info = sourceInfoProvider.GetSynchronizer(sourceView.Columns[n]) as IGridColumnModelSynchronizer;
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
                CustomModelSynchronizerEventArgs args = new CustomModelSynchronizerEventArgs(component);
                CustomModelSynchronizer(this, args);
                return args.ModelSynchronizer;
            }
            return null;
        }
        #endregion

        private ErrorMessages _errorMessages;
        private BaseGridController _gridController;
        private Boolean _skipMakeRowVisible;
        public XafLayoutView() { }
        public XafLayoutView(GridControl ownerGrid)
            : base(ownerGrid) { }
        internal void SuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit is RepositoryItemLookupEdit) {
                    //((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = false;
                }
            }
        }
        internal void CancelSuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit is RepositoryItemLookupEdit) {
                    //((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = true;
                }
            }
        }
        private object GetFocusedObject() {
            return XtraGridUtils.GetFocusedRowObject(this);
        }
        protected override BaseView CreateInstance() {
            var view = new XafLayoutView();
            view.SetGridControl(GridControl);
            return view;
        }
        protected override void RaiseShownEditor() {
            var gridInplaceEdit = ActiveEditor as IGridInplaceEdit;
            if (gridInplaceEdit != null) {
                if (GetFocusedObject() is IXPSimpleObject) {
                    (gridInplaceEdit).GridEditingObject = GetFocusedObject();
                }
            }
            base.RaiseShownEditor();
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
        protected virtual void OnCustomCreateFilterColumnCollection(CustomCreateFilterColumnCollectionEventArgs args) {
            if (CustomCreateFilterColumnCollection != null) {
                CustomCreateFilterColumnCollection(this, args);
            }
        }
        protected override FilterColumnCollection CreateFilterColumnCollection() {
            var args = new CustomCreateFilterColumnCollectionEventArgs();
            OnCustomCreateFilterColumnCollection(args);
            return args.FilterColumnCollection ?? (args.FilterColumnCollection = base.CreateFilterColumnCollection());
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
            ex.ExceptionMode = String.IsNullOrEmpty(ex.ErrorText) ? ExceptionMode.NoAction : ExceptionMode.ThrowException;
            base.RaiseInvalidRowException(ex);
        }

        protected override void OnActiveEditor_MouseDown(object sender, MouseEventArgs e) {
            if (ActiveEditor != null) {
                base.OnActiveEditor_MouseDown(sender, e);
            }
        }
        protected override BaseGridController CreateDataController() {
            _gridController = base.CreateDataController();
            return _gridController;
        }
        protected override FilterCustomDialog CreateCustomFilterDialog(GridColumn column) {
            if (!OptionsFilter.UseNewCustomFilterDialog) {
                return new FilterCustomDialog(column);
            }
            return new FilterCustomDialog2(column, Columns);
        }
        protected internal void CancelCurrentRowEdit() {
            if ((_gridController != null) && !_gridController.IsDisposed
                && (ActiveEditor != null) && (_gridController.IsCurrentRowEditing || _gridController.IsCurrentRowModified)) {
                _gridController.CancelCurrentRowEdit();
            }
        }
        protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
            if (!_skipMakeRowVisible) {
                base.MakeRowVisibleCore(rowHandle, invalidate);
            }
        }
        protected internal Boolean SkipMakeRowVisible {
            get { return _skipMakeRowVisible; }
            set { _skipMakeRowVisible = value; }
        }
        public override void ShowFilterEditor(GridColumn defaultColumn) {
            RaiseFilterEditorPopup();
            SuppressInvalidCastException();
            base.ShowFilterEditor(defaultColumn);
            CancelSuppressInvalidCastException();
            RaiseFilterEditorClosed();
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
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }
        public event EventHandler FilterEditorPopup;
        public event EventHandler FilterEditorClosed;
        public event EventHandler<CustomCreateFilterColumnCollectionEventArgs> CustomCreateFilterColumnCollection;
    }
    public class LayoutViewColumnWrapper : XafGridColumnWrapper {
        public LayoutViewColumnWrapper(GridColumn column, IGridColumnModelSynchronizer gridColumnInfo)
            : base(column, gridColumnInfo) {
        }
        public override IList<SummaryType> Summary {
            get { return null; }
            set { }
        }
        public override string SummaryFormat {
            get { return ""; }
            set {  }
        }
        public override int GroupIndex {
            get { return -1; }
            set {  }
        }
        public override DateTimeGroupInterval GroupInterval {
            get { return DateTimeGroupInterval.None; }
            set {  }
        }
        public override bool AllowGroupingChange {
            get { return false; }
            set {  }
        }
        public override bool AllowSummaryChange {
            get { return false; }
            set {  }
        }
        public override bool Visible {
            get {
                return VisibleIndex > 0;
            }
        }
        public override string ToolTip {
            get { return String.Empty; }
            set { }
        }
        public override bool ShowInCustomizationForm {
            get { return true; }
            set { }
        }
    }
    //TODO Crimp see GridViewModelSynchronizer
    public class LayoutViewModelSynchronizer : ModelSynchronizer<DevExpress.XtraGrid.Views.Layout.LayoutView, IModelListView> {
        public LayoutViewModelSynchronizer(LayoutViewListEditorBase listEditor, IModelListView modelNode)
            : base(listEditor.XafLayoutView, modelNode) {
        }
        protected override void ApplyModelCore() {
            var modelListViewShowFindPanel = Model as IModelListViewShowFindPanel;
            if (modelListViewShowFindPanel != null) {
                if ((modelListViewShowFindPanel).ShowFindPanel) {
                    Control.ShowFindPanel();
                }
                else {
                    Control.HideFindPanel();
                }
            }
        }


        public static bool IsNullableType(Type theType) {
            if (theType.IsGenericType) {
                var genericTypeDefinition = theType.GetGenericTypeDefinition();
                if (genericTypeDefinition != null) return (genericTypeDefinition == typeof(Nullable<>));
            }
            return false;
        }


        public override void SynchronizeModel() {
            var modelListViewShowFindPanel = Model as IModelListViewShowFindPanel;
            if (modelListViewShowFindPanel != null) {
                (modelListViewShowFindPanel).ShowFindPanel = Control.IsFindPanelVisible;
            }
        }
    }
    public class LayoutViewListEditorSynchronizer : ListEditorModelSynchronizer {
        public LayoutViewListEditorSynchronizer(LayoutViewListEditorBase editor)
            : base(editor, new List<IModelSynchronizable>()) {
            ModelSynchronizerList.Add(new GridViewColumnsModelSynchronizer(editor, editor.Model));
            ModelSynchronizerList.Add(new LayoutViewModelSynchronizer(editor, editor.Model));
        }
    }
    internal class CancelEventArgsAppearanceAdapter : IAppearanceEnabled {
        private readonly CancelEventArgs _cancelEdit;
        public CancelEventArgsAppearanceAdapter(CancelEventArgs cancelEdit) {
            _cancelEdit = cancelEdit;
        }
        #region IAppearanceEnabled Members
        public void ResetEnabled() {

        }

        public bool Enabled {
            get { return !_cancelEdit.Cancel; }
            set { _cancelEdit.Cancel = !value; }
        }
        #endregion
        #region IAppearanceItem Members
        public object Data {
            get { return _cancelEdit; }
        }
        #endregion
    }
    internal class AppearanceObjectAdapterWithReset : AppearanceObjectAdapter {
        private readonly AppearanceObject _appearanceObject;
        public AppearanceObjectAdapterWithReset(AppearanceObject appearanceObject)
            : base(appearanceObject) {
            _appearanceObject = appearanceObject;
        }
        public void ResetAppearance() {
            _appearanceObject.Reset();
        }
    }
    public class LayoutViewAutoScrollHelper {
        public LayoutViewAutoScrollHelper(DevExpress.XtraGrid.Views.Layout.LayoutView view) {
            _fGrid = view.GridControl;
            _fView = view;
            _fScrollInfo = new ScrollInfo(this, view);
        }

        private readonly GridControl _fGrid;
        private readonly DevExpress.XtraGrid.Views.Layout.LayoutView _fView;
        private readonly ScrollInfo _fScrollInfo;
        public int ThresholdInner = 20;
        public int ThresholdOutter = 100;
        public int HorizontalScrollStep = 10;
        public int ScrollTimerInterval {
            get {
                return _fScrollInfo.ScrollTimer.Interval;
            }
            set {
                _fScrollInfo.ScrollTimer.Interval = value;
            }
        }

        public void ScrollIfNeeded() {
            Point pt = _fGrid.PointToClient(Control.MousePosition);
            var viewInfo = ((LayoutViewInfo)_fView.GetViewInfo());
            Rectangle rect = viewInfo.ViewRects.CardsRect;
            _fScrollInfo.GoLeft = (pt.X > rect.Left - ThresholdOutter) && (pt.X < rect.Left + ThresholdInner);
            _fScrollInfo.GoRight = (pt.X > rect.Right - ThresholdInner) && (pt.X < rect.Right + ThresholdOutter);
            _fScrollInfo.GoUp = (pt.Y < rect.Top + ThresholdInner) && (pt.Y > rect.Top - ThresholdOutter);
            _fScrollInfo.GoDown = (pt.Y > rect.Bottom - ThresholdInner) && (pt.Y < rect.Bottom + ThresholdOutter);
        }

        internal class ScrollInfo {
            internal Timer ScrollTimer;
            private readonly DevExpress.XtraGrid.Views.Layout.LayoutView _view;
            private bool _left;
            private bool _right;
            private bool _up;
            private bool _down;

            private readonly LayoutViewAutoScrollHelper _owner;
            public ScrollInfo(LayoutViewAutoScrollHelper owner, DevExpress.XtraGrid.Views.Layout.LayoutView view) {
                _owner = owner;
                _view = view;
                ScrollTimer = new Timer { Interval = 500 };
                ScrollTimer.Tick += scrollTimer_Tick;
            }
            public bool GoLeft {
                get { return _left; }
                set {
                    if (_left != value) {
                        _left = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoRight {
                get { return _right; }
                set {
                    if (_right != value) {
                        _right = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoUp {
                get { return _up; }
                set {
                    if (_up != value) {
                        _up = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoDown {
                get { return _down; }
                set {
                    if (_down != value) {
                        _down = value;
                        CalcInfo();
                    }
                }
            }
            private void scrollTimer_Tick(object sender, EventArgs e) {
                _owner.ScrollIfNeeded();

                if (GoDown)
                    _view.VisibleRecordIndex++;
                if (GoUp)
                    _view.VisibleRecordIndex--;
                if (GoLeft)
                    _view.VisibleRecordIndex--;
                if (GoRight)
                    _view.VisibleRecordIndex++;

                if (_view.VisibleRecordIndex == 0 || _view.VisibleRecordIndex == _view.RowCount - 1)
                    ScrollTimer.Stop();
            }
            void CalcInfo() {
                if (!(GoDown && GoLeft && GoRight && GoUp))
                    ScrollTimer.Stop();

                if (GoDown || GoLeft || GoRight || GoUp)
                    ScrollTimer.Start();
            }
        }
    }

    public abstract class LayoutViewListEditorBase : WinColumnsListEditor, ILookupListEditor, ILookupEditProvider {
        private int _mouseDownTime;
        private int _mouseUpTime;
        private bool _activatedByMouse;
        private RepositoryItem _activeEditor;
        private Boolean _processSelectedItemBySingleClick;
        private Boolean _trackMousePosition;
        private bool _selectedItemExecuting;
        private LayoutViewAutoScrollHelper _autoScrollHelper;

        internal LayoutViewListEditorBase(IModelListView model)
            : base(model) {
        }

        public XafLayoutView XafLayoutView {
            get {
                return (XafLayoutView)ColumnView;
            }
        }
        private void layoutView_CustomCardCaptionImage(object sender, DevExpress.XtraGrid.Views.Layout.Events.LayoutViewCardCaptionImageEventArgs e) {
            e.CaptionImageLocation = GroupElementLocation.BeforeText;
            e.Image = ImageLoader.Instance.GetImageInfo(ViewImageNameHelper.GetImageName(Model)).Image;
        }
        private void layoutView_MouseMove(object sender, MouseEventArgs e) {
            if (_trackMousePosition)
                _autoScrollHelper.ScrollIfNeeded();
        }
        private void layoutView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e) {
            if (e.RowHandle == GridControl.AutoFilterRowHandle && e.Column.OptionsFilter.FilterBySortField != DefaultBoolean.False && !String.IsNullOrEmpty(e.Column.FieldNameSortGroup) && e.Column.FieldName != e.Column.FieldNameSortGroup) {
                e.RepositoryItem = new RepositoryItemStringEdit();
            }
        }
        protected override void SubscribeGridViewEvents() {
            base.SubscribeGridViewEvents();
            XafLayoutView.ShowingEditor += layoutView_EditorShowing;
            XafLayoutView.ShownEditor += layoutView_ShownEditor;
            XafLayoutView.HiddenEditor += layoutView_HiddenEditor;
            XafLayoutView.MouseDown += layoutView_MouseDown;
            XafLayoutView.MouseUp += layoutView_MouseUp;
            XafLayoutView.Click += layoutView_Click;
            XafLayoutView.MouseMove += layoutView_MouseMove;
            XafLayoutView.ShowCustomization += layoutView_ShowCustomization;
            XafLayoutView.HideCustomization += layoutView_HideCustomization;
            XafLayoutView.CustomFieldValueStyle += layoutView_CustomFieldValueStyle;
            XafLayoutView.CustomRowCellEdit += layoutView_CustomRowCellEdit;
            XafLayoutView.FilterEditorPopup += layoutView_FilterEditorPopup;
            XafLayoutView.FilterEditorClosed += layoutView_FilterEditorClosed;
            XafLayoutView.CustomCardCaptionImage += layoutView_CustomCardCaptionImage;
        }
        protected override void UnsubscribeGridViewEvents() {
            base.UnsubscribeGridViewEvents();
            XafLayoutView.ShowingEditor -= layoutView_EditorShowing;
            XafLayoutView.ShownEditor -= layoutView_ShownEditor;
            XafLayoutView.HiddenEditor -= layoutView_HiddenEditor;
            XafLayoutView.MouseDown -= layoutView_MouseDown;
            XafLayoutView.MouseUp -= layoutView_MouseUp;
            XafLayoutView.Click -= layoutView_Click;
            XafLayoutView.MouseMove -= layoutView_MouseMove;
            XafLayoutView.ShowCustomization -= layoutView_ShowCustomization;
            XafLayoutView.HideCustomization -= layoutView_HideCustomization;
            XafLayoutView.CustomFieldValueStyle -= layoutView_CustomFieldValueStyle;
            XafLayoutView.CustomRowCellEdit -= layoutView_CustomRowCellEdit;
            XafLayoutView.FilterEditorPopup -= layoutView_FilterEditorPopup;
            XafLayoutView.FilterEditorClosed -= layoutView_FilterEditorClosed;
            XafLayoutView.CustomCardCaptionImage += layoutView_CustomCardCaptionImage;
        }
        protected override void SetupGridView() {
            base.SetupGridView();
            _autoScrollHelper = new LayoutViewAutoScrollHelper(XafLayoutView);
            XafLayoutView.TemplateCard = new LayoutViewCard();
            XafLayoutView.CardMinSize = new Size(400, 200);
            XafLayoutView.ErrorMessages = ErrorMessages;
        }
        private void layoutView_CustomFieldValueStyle(object sender, DevExpress.XtraGrid.Views.Layout.Events.LayoutViewFieldValueStyleEventArgs e) {
            IGridColumnModelSynchronizer columnInfo = GetColumnModelSynchronizer(e.Column);
            string propertyName = columnInfo != null ? columnInfo.PropertyName : e.Column.FieldName;
            OnCustomizeAppearance(propertyName, new AppearanceObjectAdapter(e.Appearance), e.RowHandle);
        }
        private void layoutView_HideCustomization(object sender, EventArgs e) {
            OnEndCustomization();
        }
        private void layoutView_ShowCustomization(object sender, EventArgs e) {
            OnBeginCustomization();
        }
        private void layoutView_FilterEditorPopup(object sender, EventArgs e) {
            OnBeginCustomization();
        }
        private void layoutView_FilterEditorClosed(object sender, EventArgs e) {
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
        private void grid_DoubleClick(object sender, EventArgs e) {
            ProcessMouseClick(e);
        }
        private void layoutView_Click(object sender, EventArgs e) {
            if (_processSelectedItemBySingleClick) {
                ProcessMouseClick(e);
            }
        }
        private void layoutView_EditorShowing(object sender, CancelEventArgs e) {
            _activeEditor = null;
            string propertyName = ColumnView.FocusedColumn.FieldName;
            RepositoryItem edit = ColumnView.FocusedColumn.ColumnEdit;
            OnCustomizeAppearance(propertyName, new CancelEventArgsAppearanceAdapter(e), ColumnView.FocusedRowHandle);
            if (!e.Cancel) {
                if (edit != null) {
                    OnCustomizeAppearance(propertyName, new AppearanceObjectAdapterWithReset(edit.Appearance), ColumnView.FocusedRowHandle);
                    edit.MouseDown += Editor_MouseDown;
                    edit.MouseUp += Editor_MouseUp;
                    var buttonEdit = edit as RepositoryItemButtonEdit;
                    if (buttonEdit != null) {
                        buttonEdit.ButtonPressed += ButtonEdit_ButtonPressed;
                    }
                    var spinEdit = edit as RepositoryItemBaseSpinEdit;
                    if (spinEdit != null) {
                        spinEdit.Spin += SpinEdit_Spin;
                    }
                    edit.KeyDown += Editor_KeyDown;
                    _activeEditor = edit;
                }
            }
        }
        private void layoutView_ShownEditor(object sender, EventArgs e) {
            if (PopupMenu != null) {
                PopupMenu.ResetPopupContextMenuSite();
            }
            var popupEdit = ColumnView.ActiveEditor as PopupBaseEdit;
            if (popupEdit != null && _activatedByMouse && popupEdit.Properties.ShowDropDown != ShowDropDown.Never) {
                popupEdit.ShowPopup();
            }
            _activatedByMouse = false;
            var editor = ColumnView.ActiveEditor as LookupEdit;
            if (editor != null) {
                OnLookupEditCreated(editor);
            }
        }
        private void layoutView_HiddenEditor(object sender, EventArgs e) {
            if (PopupMenu != null) {
                PopupMenu.SetupPopupContextMenuSite();
            }
            var editor = ColumnView.ActiveEditor as LookupEdit;
            if (editor != null) {
                OnLookupEditHide(editor);
            }
            if (_activeEditor != null) {
                _activeEditor.KeyDown -= Editor_KeyDown;
                _activeEditor.MouseDown -= Editor_MouseDown;
                _activeEditor.MouseUp -= Editor_MouseUp;
                var buttonEdit = _activeEditor as RepositoryItemButtonEdit;
                if (buttonEdit != null) {
                    buttonEdit.ButtonPressed -= ButtonEdit_ButtonPressed;
                }
                var spinEdit = _activeEditor as RepositoryItemBaseSpinEdit;
                if (spinEdit != null) {
                    spinEdit.Spin -= SpinEdit_Spin;
                }
                _activeEditor = null;
            }
        }
        private void layoutView_MouseDown(object sender, MouseEventArgs e) {
            var view = (DevExpress.XtraGrid.Views.Layout.LayoutView)sender;
            LayoutViewHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
            _mouseDownTime = hi.RowHandle >= 0 ? Environment.TickCount : 0;
            _activatedByMouse = true;
        }
        private void layoutView_MouseUp(object sender, MouseEventArgs e) {
            _mouseUpTime = Environment.TickCount;
        }
        private void Editor_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Int32 currentTime = Environment.TickCount;
                if ((_mouseDownTime <= _mouseUpTime) && (_mouseUpTime <= currentTime) && (currentTime - _mouseDownTime < SystemInformation.DoubleClickTime)) {
                    OnProcessSelectedItem();
                    _mouseDownTime = 0;
                }
            }
        }
        private void Editor_MouseUp(object sender, MouseEventArgs e) {
            _mouseUpTime = Environment.TickCount;
        }
        private void Editor_KeyDown(object sender, KeyEventArgs e) {
            ProcessEditorKeyDown(e);
        }
        protected virtual void ProcessEditorKeyDown(KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if ((ColumnView.ActiveEditor != null) && ColumnView.ActiveEditor.IsModified) {
                    ColumnView.PostEditor();
                    ColumnView.UpdateCurrentRow();
                }
            }
        }

        private void SpinEdit_Spin(object sender, SpinEventArgs e) {
            _mouseDownTime = 0;
        }
        private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e) {
            _mouseDownTime = 0;
        }

        protected virtual void ProcessMouseClick(EventArgs e) {
            if (!_selectedItemExecuting) {
                if (ColumnView.FocusedRowHandle >= 0) {
                    DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(Grid, e);
                    LayoutViewHitInfo hitInfo = XafLayoutView.CalcHitInfo(args.Location);
                    if (hitInfo.InCard
                        && (hitInfo.HitTest == LayoutViewHitTest.FieldCaption
                            || hitInfo.HitTest == LayoutViewHitTest.CardCaption
                            || hitInfo.HitTest == LayoutViewHitTest.Field
                            || hitInfo.HitTest == LayoutViewHitTest.FieldValue
                            || hitInfo.HitTest == LayoutViewHitTest.LayoutItem)
                        ) {
                        args.Handled = true;
                        OnProcessSelectedItem();
                    }
                }
            }
        }
        protected override void SubscribeToGridEvents() {
            base.SubscribeToGridEvents();
            Grid.DoubleClick += grid_DoubleClick;
        }
        protected override void UnsubscribeFromGridEvents() {
            base.UnsubscribeFromGridEvents();
            Grid.DoubleClick -= grid_DoubleClick;
        }
        protected override void OnProcessSelectedItem() {
            _selectedItemExecuting = true;
            try {
                if ((ColumnView != null) && (ColumnView.ActiveEditor != null)) {
                    BindingHelper.EndCurrentEdit(Grid);
                }
                base.OnProcessSelectedItem();
            }
            finally {
                _selectedItemExecuting = false;
            }
        }
        protected override void OnCreateCustomColumn(object sender, CreateCustomColumnEventArgs e) {
            base.OnCreateCustomColumn(sender, e);
            if (e.Column == null) {
                e.Column = new LayoutViewColumn();
                e.GridColumnInfo = new LayoutViewColumnInfo(e.ModelColumn, e.ObjectTypeInfo, IsInstantFeedbackMode, HasProtectedContent(e.ModelColumn.PropertyName));
            }
        }
        protected override void OnCustomizeGridColumn(object sender, CustomizeGridColumnEventArgs e) {
            base.OnCustomizeGridColumn(sender, e);
            LayoutViewColumn column = (LayoutViewColumn)e.GridColumn;
            if (column.LayoutViewField == null) {
                column.LayoutViewField = new LayoutViewField();
            }
            column.LayoutViewField.Name = column.FieldName;
            column.LayoutViewField.ColumnName = column.FieldName;
            if (!XafLayoutView.TemplateCard.Items.Contains(column.LayoutViewField))
                XafLayoutView.TemplateCard.Add(column.LayoutViewField);
        }
        public override object FocusedObject {
            get {
                object result = null;
                if (ColumnView != null)
                    result = XtraGridUtils.GetFocusedRowObject(CollectionSource, ColumnView);
                return result;
            }
            set {
                if (value != null && value != DBNull.Value && ColumnView != null && DataSource != null) {
                    XtraGridUtils.SelectRowByHandle(ColumnView, ColumnView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(ColumnView))
                        XafLayoutView.ExpandCard(ColumnView.FocusedRowHandle);
                }
            }
        }
        public override SelectionType SelectionType {
            get { return SelectionType.Full; }
        }

        public override bool CanShowPopupMenu(Point position) {
            LayoutViewHitTest hitTest = XafLayoutView.CalcHitInfo(Grid.PointToClient(position)).HitTest;
            return
                ((hitTest == LayoutViewHitTest.Card)
                 || (hitTest == LayoutViewHitTest.Field)
                 || (hitTest == LayoutViewHitTest.FieldValue)
                 || (hitTest == LayoutViewHitTest.FieldCaption)
                 || (hitTest == LayoutViewHitTest.CardCaption)
                 || (hitTest == LayoutViewHitTest.LayoutItem)
                 || (hitTest == LayoutViewHitTest.None));
        }
        #region ILookupListEditor Members
        public Boolean ProcessSelectedItemBySingleClick {
            get { return _processSelectedItemBySingleClick; }
            set { _processSelectedItemBySingleClick = value; }
        }
        public Boolean TrackMousePosition {
            get { return _trackMousePosition; }
            set { _trackMousePosition = value; }
        }
        public event EventHandler BeginCustomization;
        public event EventHandler EndCustomization;
        #endregion
        protected override void ApplyHtmlFormatting(bool htmlFormattingEnabled) {
            base.ApplyHtmlFormatting(htmlFormattingEnabled);
            XafLayoutView.Appearance.HeaderPanel.TextOptions.WordWrap = htmlFormattingEnabled ? WordWrap.Wrap : WordWrap.Default;
        }
        #region ILookupEditProvider Members
        private event EventHandler<LookupEditProviderEventArgs> LookupEditCreated;
        private event EventHandler<LookupEditProviderEventArgs> LookupEditHide;
        protected void OnLookupEditCreated(LookupEdit control) {
            if (LookupEditCreated != null) {
                LookupEditCreated(this, new LookupEditProviderEventArgs(control));
            }
        }
        protected void OnLookupEditHide(LookupEdit control) {
            if (LookupEditHide != null) {
                LookupEditHide(this, new LookupEditProviderEventArgs(control));
            }
        }
        event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.ControlCreated {
            add { LookupEditCreated += value; }
            remove { LookupEditCreated -= value; }
        }
        event EventHandler<LookupEditProviderEventArgs> ILookupEditProvider.HideControl {
            add { LookupEditHide += value; }
            remove { LookupEditHide -= value; }
        }
        #endregion
    }
    public class LayoutViewColumnInfo : GridColumnModelSynchronizer {
        public LayoutViewColumnInfo(IModelColumn modelColumn, ITypeInfo objectTypeInfo, bool isAsyncServerMode, bool isProtectedColumn)
            : base(modelColumn, objectTypeInfo, isAsyncServerMode, isProtectedColumn) {
        }
        public override XafGridColumnWrapper CreateColumnWrapper(GridColumn column) {
            return new LayoutViewColumnWrapper(column, this);
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class CustomCreateFilterColumnCollectionEventArgs : EventArgs {
        public FilterColumnCollection FilterColumnCollection { get; set; }
    }
    public class LayoutViewColumnChooserController : ColumnChooserControllerBase {
        private LayoutViewField _selectedColumn;
        private DevExpress.XtraGrid.Views.Layout.LayoutView _layoutView;
        private LayoutControl _layoutControl;
        private LayoutViewCustomizationForm _customizationFormCore;
        private LayoutViewListEditorBase ListEditor {
            get { return ((DevExpress.ExpressApp.ListView)View).Editor as LayoutViewListEditorBase; }
        }
        private void columnChooser_SelectedColumnChanged(object sender, EventArgs e) {
            if (_selectedColumn != null) {
                _selectedColumn.ImageIndex = -1;
            }
            _selectedColumn = ((ListBoxControl)ActiveListBox).SelectedItem as LayoutViewField;
            if (_selectedColumn != null) {
                _selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
            }
            RemoveButton.Enabled = _selectedColumn != null;
        }
        private void layoutView_ShowCustomization(object sender, EventArgs e) {
            CustomizationForm.VisibleChanged += CustomizationForm_VisibleChanged;
        }
        private void CustomizationForm_VisibleChanged(object sender, EventArgs e) {
            ((Control)sender).VisibleChanged -= CustomizationForm_VisibleChanged;
            if (((Control)sender).Visible) {
                _layoutControl = new List<LayoutControl>(FindNestedControls<LayoutControl>(CustomizationForm))[3];
                InsertButtons();
                AddButton.Text += " (TODO)";
                _selectedColumn = null;
                ((ListBoxControl)ActiveListBox).SelectedItem = null;
                ActiveListBox.KeyDown += ActiveListBox_KeyDown;
                ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
                _layoutView.Images = GridPainter.Indicator;
            }
        }
        private void layoutView_HideCustomization(object sender, EventArgs e) {
            DeleteButtons();
            if (_selectedColumn != null) {
                _selectedColumn.ImageIndex = -1;
            }
            _layoutView.Images = null;
            ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
            ActiveListBox.KeyDown += ActiveListBox_KeyDown;
            _layoutControl = null;
            _customizationFormCore = null;
            _selectedColumn = null;
        }
        private void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelectedColumn();
            }
        }
        protected LayoutViewCustomizationForm CustomizationForm {
            get {
                return _customizationFormCore ??
                       (_customizationFormCore =
                        typeof(DevExpress.XtraGrid.Views.Layout.LayoutView).GetProperty("CustomizationForm",
                                                       System.Reflection.BindingFlags.Instance |
                                                       System.Reflection.BindingFlags.NonPublic).GetValue(_layoutView,
                                                                                                          null) as
                        LayoutViewCustomizationForm);
            }
        }
        protected override Control ActiveListBox {
            get {
                return _layoutControl.Controls[4];
            }
        }
        private static IEnumerable<T> FindNestedControls<T>(Control container) where T : Control {
            //            if (container.Controls != null)
            foreach (Control item in container.Controls) {
                var controls = item as T;
                if (controls != null)
                    yield return controls;
                foreach (T child in FindNestedControls<T>(item))
                    yield return child;
            }
        }
        protected override List<string> GetUsedProperties() {
            return ListEditor.Model.Columns.Select(columnInfoNodeWrapper => columnInfoNodeWrapper.PropertyName).ToList();
        }

        protected override ITypeInfo DisplayedTypeInfo {
            get { return View.ObjectTypeInfo; }
        }
        //TODO: Implement adding new properties into the customization form.
        protected override void AddColumn(string propertyName) {
            IModelColumn columnInfo = FindColumnModelByPropertyName(propertyName);
            if (columnInfo == null) {
                columnInfo = ListEditor.Model.Columns.AddNode<IModelColumn>();
                columnInfo.Id = propertyName;
                columnInfo.PropertyName = propertyName;
                columnInfo.Index = -1;
                var wrapper = ListEditor.AddColumn(columnInfo) as XafGridColumnWrapper;
                if (wrapper != null && wrapper.Column is LayoutViewColumn && ((LayoutViewColumn)wrapper.Column).LayoutViewField != null) {
                    ((ListBoxControl)ActiveListBox).Items.Add(((LayoutViewColumn)wrapper.Column).LayoutViewField);
                }
            }
            else {
                throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddDuplicateProperty, propertyName));
            }
        }
        protected IModelColumn FindColumnModelByPropertyName(string propertyName) {
            IModelColumn columnInfo = null;
            foreach (IModelColumn colInfo in ListEditor.Model.Columns) {
                if (colInfo.PropertyName == propertyName) {
                    columnInfo = colInfo;
                    break;
                }
            }
            return columnInfo;
        }
        protected override void RemoveSelectedColumn() {
            var field = ((ListBoxControl)ActiveListBox).SelectedItem as LayoutViewField;
            if (field != null) {
                LayoutViewColumnWrapper columnInfo = (from LayoutViewColumn item in _layoutView.Columns where item.FieldName == field.FieldName select ListEditor.FindColumn(GetColumnInfo(item, _layoutView).PropertyName) as LayoutViewColumnWrapper).FirstOrDefault();
                if (columnInfo != null)
                    ListEditor.RemoveColumn(columnInfo);
                ((ListBoxControl)ActiveListBox).Items.Remove(field);
            }
        }
        private IGridColumnModelSynchronizer GetColumnInfo(GridColumn column, DevExpress.XtraGrid.Views.Base.ColumnView view) {
            IGridColumnModelSynchronizer result = null;
            var holder = view as IModelSynchronizersHolder;
            if (holder != null) {
                result = holder.GetSynchronizer(column) as IGridColumnModelSynchronizer;
            }
            return result;
        }
        protected override void AddButtonsToCustomizationForm() {
            _layoutControl.Controls.Add(RemoveButton);
            _layoutControl.Controls.Add(AddButton);

            var hiddenItemsGroup = ((LayoutControlGroup)_layoutControl.Items[0]);
            LayoutControlItem addButtonLayoutItem = hiddenItemsGroup.AddItem();
            addButtonLayoutItem.Control = AddButton;
            addButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 0);
            addButtonLayoutItem.TextVisible = false;

            LayoutControlItem removeButtonLayoutItem = hiddenItemsGroup.AddItem();
            removeButtonLayoutItem.Control = RemoveButton;
            removeButtonLayoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 0, 5);
            removeButtonLayoutItem.TextVisible = false;
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated();
            SubscribeLayoutViewEvents();
        }
        private void SubscribeLayoutViewEvents() {
            if (ListEditor != null) {
                _layoutView = (DevExpress.XtraGrid.Views.Layout.LayoutView)ListEditor.ColumnView;
                _layoutView.ShowCustomization += layoutView_ShowCustomization;
                _layoutView.HideCustomization += layoutView_HideCustomization;
            }
        }
        protected override void OnDeactivated() {
            UnsubscribeLayoutViewEvents();
            _selectedColumn = null;
            base.OnDeactivated();
        }
        private void UnsubscribeLayoutViewEvents() {
            if (_layoutView != null) {
                _layoutView.ShowCustomization -= layoutView_ShowCustomization;
                _layoutView.HideCustomization -= layoutView_HideCustomization;
                _layoutView = null;
            }
        }
        public LayoutViewColumnChooserController() {
            TypeOfView = typeof(DevExpress.ExpressApp.ListView);
        }
    }

}