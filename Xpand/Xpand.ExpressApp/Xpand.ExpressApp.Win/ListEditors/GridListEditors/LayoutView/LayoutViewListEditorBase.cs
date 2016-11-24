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
using DevExpress.ExpressApp.SystemModule;
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
        public Dictionary<Component, IModelSynchronizer> columnsInfoCache = new Dictionary<Component, IModelSynchronizer>();
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
                    columnsInfoCache.TryGetValue(component, out result);
                }
            }
            return result;
        }
        void IModelSynchronizersHolder.RegisterSynchronizer(Component component, IModelSynchronizer modelSynchronizer) {
            columnsInfoCache.Add(component, modelSynchronizer);
        }
        void IModelSynchronizersHolder.RemoveSynchronizer(Component component) {
            if (component != null && columnsInfoCache.ContainsKey(component)) {
                columnsInfoCache.Remove(component);
            }
        }
        void IModelSynchronizersHolder.AssignSynchronizers(DevExpress.XtraGrid.Views.Base.ColumnView sourceView) {
            IModelSynchronizersHolder current = (IModelSynchronizersHolder)this;
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

        private ErrorMessages errorMessages;
        private BaseGridController gridController;
        private Boolean skipMakeRowVisible;
        public XafLayoutView() { }
        public XafLayoutView(GridControl ownerGrid)
            : base(ownerGrid) { }
        internal void SuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
                    //((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = false;
                }
            }
        }
        internal void CancelSuppressInvalidCastException() {
            foreach (GridColumn column in Columns) {
                if (column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
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
            if (errorMessages != null) {
                object listItem = GetRow(rowHandle);
                ErrorMessage message = column == null ? errorMessages.GetMessages(listItem) : errorMessages.GetMessage(column.FieldName, listItem);
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
            gridController = base.CreateDataController();
            return gridController;
        }
        protected override FilterCustomDialog CreateCustomFilterDialog(GridColumn column) {
            if (!OptionsFilter.UseNewCustomFilterDialog) {
                return new FilterCustomDialog(column);
            }
            return new FilterCustomDialog2(column, Columns);
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
            get { return errorMessages; }
            set { errorMessages = value; }
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
            set { ; }
        }
        public override int GroupIndex {
            get { return -1; }
            set { ; }
        }
        public override DateTimeGroupInterval GroupInterval {
            get { return DateTimeGroupInterval.None; }
            set { ; }
        }
        public override bool AllowGroupingChange {
            get { return false; }
            set { ; }
        }
        public override bool AllowSummaryChange {
            get { return false; }
            set { ; }
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
        private readonly LayoutViewListEditorBase listEditor;
        public LayoutViewModelSynchronizer(LayoutViewListEditorBase listEditor, IModelListView modelNode)
            : base(listEditor.XafLayoutView, modelNode) {
            this.listEditor = listEditor;
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
            : base(editor, new List<DevExpress.ExpressApp.Model.IModelSynchronizable>()) {
            ModelSynchronizerList.Add(new GridViewColumnsModelSynchronizer(editor, editor.Model));
            ModelSynchronizerList.Add(new LayoutViewModelSynchronizer(editor, editor.Model));
        }
    }
    internal class CancelEventArgsAppearanceAdapter : IAppearanceEnabled {
        private readonly CancelEventArgs cancelEdit;
        public CancelEventArgsAppearanceAdapter(CancelEventArgs cancelEdit) {
            this.cancelEdit = cancelEdit;
        }
        #region IAppearanceEnabled Members
        public void ResetEnabled() {

        }

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
    internal class AppearanceObjectAdapterWithReset : AppearanceObjectAdapter {
        private readonly AppearanceObject appearanceObject;
        public AppearanceObjectAdapterWithReset(AppearanceObject appearanceObject)
            : base(appearanceObject) {
            this.appearanceObject = appearanceObject;
        }
        public void ResetAppearance() {
            appearanceObject.Reset();
        }
    }
    public class LayoutViewAutoScrollHelper {
        public LayoutViewAutoScrollHelper(DevExpress.XtraGrid.Views.Layout.LayoutView view) {
            fGrid = view.GridControl;
            fView = view;
            fScrollInfo = new ScrollInfo(this, view);
        }

        readonly GridControl fGrid;
        readonly DevExpress.XtraGrid.Views.Layout.LayoutView fView;
        readonly ScrollInfo fScrollInfo;
        public int ThresholdInner = 20;
        public int ThresholdOutter = 100;
        public int HorizontalScrollStep = 10;
        public int ScrollTimerInterval {
            get {
                return fScrollInfo.scrollTimer.Interval;
            }
            set {
                fScrollInfo.scrollTimer.Interval = value;
            }
        }

        public void ScrollIfNeeded() {
            Point pt = fGrid.PointToClient(Control.MousePosition);
            var viewInfo = ((LayoutViewInfo)fView.GetViewInfo());
            Rectangle rect = viewInfo.ViewRects.CardsRect;
            fScrollInfo.GoLeft = (pt.X > rect.Left - ThresholdOutter) && (pt.X < rect.Left + ThresholdInner);
            fScrollInfo.GoRight = (pt.X > rect.Right - ThresholdInner) && (pt.X < rect.Right + ThresholdOutter);
            fScrollInfo.GoUp = (pt.Y < rect.Top + ThresholdInner) && (pt.Y > rect.Top - ThresholdOutter);
            fScrollInfo.GoDown = (pt.Y > rect.Bottom - ThresholdInner) && (pt.Y < rect.Bottom + ThresholdOutter);
        }

        internal class ScrollInfo {
            internal Timer scrollTimer;
            readonly DevExpress.XtraGrid.Views.Layout.LayoutView view;
            bool left, right, up, down;

            readonly LayoutViewAutoScrollHelper owner;
            public ScrollInfo(LayoutViewAutoScrollHelper owner, DevExpress.XtraGrid.Views.Layout.LayoutView view) {
                this.owner = owner;
                this.view = view;
                scrollTimer = new Timer { Interval = 500 };
                scrollTimer.Tick += scrollTimer_Tick;
            }
            public bool GoLeft {
                get { return left; }
                set {
                    if (left != value) {
                        left = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoRight {
                get { return right; }
                set {
                    if (right != value) {
                        right = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoUp {
                get { return up; }
                set {
                    if (up != value) {
                        up = value;
                        CalcInfo();
                    }
                }
            }
            public bool GoDown {
                get { return down; }
                set {
                    if (down != value) {
                        down = value;
                        CalcInfo();
                    }
                }
            }
            private void scrollTimer_Tick(object sender, EventArgs e) {
                owner.ScrollIfNeeded();

                if (GoDown)
                    view.VisibleRecordIndex++;
                if (GoUp)
                    view.VisibleRecordIndex--;
                if (GoLeft)
                    view.VisibleRecordIndex--;
                if (GoRight)
                    view.VisibleRecordIndex++;

                if (view.VisibleRecordIndex == 0 || view.VisibleRecordIndex == view.RowCount - 1)
                    scrollTimer.Stop();
            }
            void CalcInfo() {
                if (!(GoDown && GoLeft && GoRight && GoUp))
                    scrollTimer.Stop();

                if (GoDown || GoLeft || GoRight || GoUp)
                    scrollTimer.Start();
            }
        }
    }

    public abstract class LayoutViewListEditorBase : WinColumnsListEditor, IExportable, ILookupListEditor, IHtmlFormattingSupport, ILookupEditProvider, ISupportAppearanceCustomization {
        private int mouseDownTime;
        private int mouseUpTime;
        private bool activatedByMouse;
        private RepositoryItem activeEditor;
        private Boolean processSelectedItemBySingleClick;
        private Boolean trackMousePosition;
        private bool selectedItemExecuting;
        private LayoutViewAutoScrollHelper autoScrollHelper;

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
            if (trackMousePosition)
                autoScrollHelper.ScrollIfNeeded();
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
            autoScrollHelper = new LayoutViewAutoScrollHelper(XafLayoutView);
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
            if (processSelectedItemBySingleClick) {
                ProcessMouseClick(e);
            }
        }
        private void layoutView_EditorShowing(object sender, CancelEventArgs e) {
            activeEditor = null;
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
                    activeEditor = edit;
                }
            }
        }
        private void layoutView_ShownEditor(object sender, EventArgs e) {
            if (PopupMenu != null) {
                PopupMenu.ResetPopupContextMenuSite();
            }
            var popupEdit = ColumnView.ActiveEditor as PopupBaseEdit;
            if (popupEdit != null && activatedByMouse && popupEdit.Properties.ShowDropDown != ShowDropDown.Never) {
                popupEdit.ShowPopup();
            }
            activatedByMouse = false;
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
            if (activeEditor != null) {
                activeEditor.KeyDown -= Editor_KeyDown;
                activeEditor.MouseDown -= Editor_MouseDown;
                activeEditor.MouseUp -= Editor_MouseUp;
                var buttonEdit = activeEditor as RepositoryItemButtonEdit;
                if (buttonEdit != null) {
                    buttonEdit.ButtonPressed -= ButtonEdit_ButtonPressed;
                }
                var spinEdit = activeEditor as RepositoryItemBaseSpinEdit;
                if (spinEdit != null) {
                    spinEdit.Spin -= SpinEdit_Spin;
                }
                activeEditor = null;
            }
        }
        private void layoutView_MouseDown(object sender, MouseEventArgs e) {
            var view = (DevExpress.XtraGrid.Views.Layout.LayoutView)sender;
            LayoutViewHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
            mouseDownTime = hi.RowHandle >= 0 ? Environment.TickCount : 0;
            activatedByMouse = true;
        }
        private void layoutView_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = Environment.TickCount;
        }
        private void Editor_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Int32 currentTime = Environment.TickCount;
                if ((mouseDownTime <= mouseUpTime) && (mouseUpTime <= currentTime) && (currentTime - mouseDownTime < SystemInformation.DoubleClickTime)) {
                    OnProcessSelectedItem();
                    mouseDownTime = 0;
                }
            }
        }
        private void Editor_MouseUp(object sender, MouseEventArgs e) {
            mouseUpTime = Environment.TickCount;
        }
        private void Editor_KeyDown(object sender, KeyEventArgs e) {
            ProcessEditorKeyDown(e);
        }
        protected virtual void ProcessEditorKeyDown(KeyEventArgs e) {
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
        private IMemberInfo FindMemberInfoForColumn(IModelColumn columnInfo) {
            return ObjectTypeInfo.FindMember(columnInfo.PropertyName);
        }
        protected virtual void ProcessMouseClick(EventArgs e) {
            if (!selectedItemExecuting) {
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
            selectedItemExecuting = true;
            try {
                if ((ColumnView != null) && (ColumnView.ActiveEditor != null)) {
                    BindingHelper.EndCurrentEdit(Grid);
                }
                base.OnProcessSelectedItem();
            }
            finally {
                selectedItemExecuting = false;
            }
        }
        protected override void OnCreateCustomColumn(object sender, CreateCustomColumnEventArgs e) {
            base.OnCreateCustomColumn(sender, e);
            if (e.Column == null) {
                e.Column = new LayoutViewColumn();
                e.GridColumnInfo = new LayoutViewColumnInfo(e.ModelColumn, e.ObjectTypeInfo, IsAsyncServerMode, HasProtectedContent(e.ModelColumn.PropertyName));
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
        protected override void ApplyHtmlFormatting(bool htmlFormattingEnabled) {
            base.ApplyHtmlFormatting(htmlFormattingEnabled);
            XafLayoutView.Appearance.HeaderPanel.TextOptions.WordWrap = htmlFormattingEnabled ? WordWrap.Wrap : WordWrap.Default;
        }
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
        private LayoutViewField selectedColumn;
        private DevExpress.XtraGrid.Views.Layout.LayoutView layoutView;
        private LayoutControl layoutControl;
        private LayoutViewCustomizationForm customizationFormCore;
        private LayoutViewListEditorBase ListEditor {
            get { return ((DevExpress.ExpressApp.ListView)View).Editor as LayoutViewListEditorBase; }
        }
        private void columnChooser_SelectedColumnChanged(object sender, EventArgs e) {
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            selectedColumn = ((ListBoxControl)ActiveListBox).SelectedItem as LayoutViewField;
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = GridPainter.IndicatorFocused;
            }
            RemoveButton.Enabled = selectedColumn != null;
        }
        private void layoutView_ShowCustomization(object sender, EventArgs e) {
            CustomizationForm.VisibleChanged += CustomizationForm_VisibleChanged;
        }
        private void CustomizationForm_VisibleChanged(object sender, EventArgs e) {
            ((Control)sender).VisibleChanged -= CustomizationForm_VisibleChanged;
            if (((Control)sender).Visible) {
                layoutControl = new List<LayoutControl>(FindNestedControls<LayoutControl>(CustomizationForm))[3];
                InsertButtons();
                AddButton.Text += " (TODO)";
                selectedColumn = null;
                ((ListBoxControl)ActiveListBox).SelectedItem = null;
                ActiveListBox.KeyDown += ActiveListBox_KeyDown;
                ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
                layoutView.Images = GridPainter.Indicator;
            }
        }
        private void layoutView_HideCustomization(object sender, EventArgs e) {
            DeleteButtons();
            if (selectedColumn != null) {
                selectedColumn.ImageIndex = -1;
            }
            layoutView.Images = null;
            ((ListBoxControl)ActiveListBox).SelectedValueChanged += columnChooser_SelectedColumnChanged;
            ActiveListBox.KeyDown += ActiveListBox_KeyDown;
            layoutControl = null;
            customizationFormCore = null;
            selectedColumn = null;
        }
        private void ActiveListBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelectedColumn();
            }
        }
        protected LayoutViewCustomizationForm CustomizationForm {
            get {
                return customizationFormCore ??
                       (customizationFormCore =
                        typeof(DevExpress.XtraGrid.Views.Layout.LayoutView).GetProperty("CustomizationForm",
                                                       System.Reflection.BindingFlags.Instance |
                                                       System.Reflection.BindingFlags.NonPublic).GetValue(layoutView,
                                                                                                          null) as
                        LayoutViewCustomizationForm);
            }
        }
        protected override Control ActiveListBox {
            get {
                return layoutControl.Controls[4];
            }
        }
        private static IEnumerable<T> FindNestedControls<T>(Control container) where T : Control {
            //            if (container.Controls != null)
            foreach (Control item in container.Controls) {
                if (item is T)
                    yield return (T)item;
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
                if (wrapper != null && wrapper.Column != null && wrapper.Column is LayoutViewColumn && ((LayoutViewColumn)wrapper.Column).LayoutViewField != null) {
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
                LayoutViewColumnWrapper columnInfo = (from LayoutViewColumn item in layoutView.Columns where item.FieldName == field.FieldName select ListEditor.FindColumn(GetColumnInfo((GridColumn)item, layoutView).PropertyName) as LayoutViewColumnWrapper).FirstOrDefault();
                if (columnInfo != null)
                    ListEditor.RemoveColumn(columnInfo);
                ((ListBoxControl)ActiveListBox).Items.Remove(field);
            }
        }
        private IGridColumnModelSynchronizer GetColumnInfo(GridColumn column, DevExpress.XtraGrid.Views.Base.ColumnView view) {
            IGridColumnModelSynchronizer result = null;
            if (view is IModelSynchronizersHolder) {
                result = ((IModelSynchronizersHolder)view).GetSynchronizer(column) as IGridColumnModelSynchronizer;
            }
            return result;
        }
        protected override void AddButtonsToCustomizationForm() {
            layoutControl.Controls.Add(RemoveButton);
            layoutControl.Controls.Add(AddButton);

            var hiddenItemsGroup = ((LayoutControlGroup)layoutControl.Items[0]);
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
                layoutView = (DevExpress.XtraGrid.Views.Layout.LayoutView)ListEditor.ColumnView;
                layoutView.ShowCustomization += layoutView_ShowCustomization;
                layoutView.HideCustomization += layoutView_HideCustomization;
            }
        }
        protected override void OnDeactivated() {
            UnsubscribeLayoutViewEvents();
            selectedColumn = null;
            base.OnDeactivated();
        }
        private void UnsubscribeLayoutViewEvents() {
            if (layoutView != null) {
                layoutView.ShowCustomization -= layoutView_ShowCustomization;
                layoutView.HideCustomization -= layoutView_HideCustomization;
                layoutView = null;
            }
        }
        public LayoutViewColumnChooserController() {
            TypeOfView = typeof(DevExpress.ExpressApp.ListView);
        }
    }

}