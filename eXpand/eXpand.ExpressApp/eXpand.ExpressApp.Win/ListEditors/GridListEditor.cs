using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.Win.ListEditors
{
    public class GridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider
    {
//        private bool activatedByMouse;
//        private RepositoryItem _activeEditor;
//        private long _mouseUpTime;
//        private long _mouseDownTime;
        private bool _hidePopupMenu;
//        bool _selectedItemExecuting;
        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;
        public new void AssignDataSourceToControl(object dataSource) {
            base.AssignDataSourceToControl(dataSource);
        }

        public void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }
        public new XafGridView GridView
        {
            get { return (XafGridView)base.GridView; }
        }
//        private void gridView_MouseDown(object sender, MouseEventArgs e)
//        {
//            var view = (GridView)sender;
//            GridHitInfo hi = view.CalcHitInfo(new Point(e.X, e.Y));
//            _mouseDownTime = hi.RowHandle >= 0 ? Environment.TickCount : 0;
//            activatedByMouse = true;
//        }

//        private void Editor_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Enter)
//            {
//                SubmitActiveEditorChanges();
//            }
//        }
//        private void SubmitActiveEditorChanges()
//        {
//            if ((GridView.ActiveEditor != null) && GridView.ActiveEditor.IsModified)
//            {
//                GridView.PostEditor();
//                GridView.UpdateCurrentRow();
//            }
//        }
//        private void Editor_MouseDown(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left)
//            {
//                Int32 currentTime = Environment.TickCount;
//                if ((_mouseDownTime <= _mouseUpTime) && (_mouseUpTime <= currentTime) && (currentTime - _mouseDownTime < SystemInformation.DoubleClickTime))
//                {
//                    OnProcessSelectedItem();
//                    _mouseDownTime = 0;
//                }
//            }
//        }
//        private void Editor_MouseUp(object sender, MouseEventArgs e)
//        {
//            _mouseUpTime = Environment.TickCount;
//        }
//        protected override object CreateControlsCore() {
//            object controlsCore = base.CreateControlsCore();
//            GridView.ShownEditor += gridView_ShownEditor;
//            GridView.HiddenEditor += gridView_HiddenEditor;
//            GridView.MouseDown+=gridView_MouseDown;
//            GridView.ShowingEditor += gridView_EditorShowing;
//            return controlsCore;
//        }
//        private void gridView_ShownEditor(object sender, EventArgs e)
//        {
//            var popupEdit = GridView.ActiveEditor as PopupBaseEdit;
//            if (popupEdit != null && activatedByMouse && popupEdit.Properties.ShowDropDown != ShowDropDown.Never)
//            {
//                popupEdit.ShowPopup();
//            }
//            activatedByMouse = false;
//        }

//        private void gridView_HiddenEditor(object sender, EventArgs e)
//        {
//            if (_activeEditor != null)
//            {
//                _activeEditor.KeyDown -= Editor_KeyDown;
//                _activeEditor.MouseDown -= Editor_MouseDown;
//                _activeEditor.MouseUp -= Editor_MouseUp;
//                var buttonEdit = _activeEditor as RepositoryItemButtonEdit;
//                if (buttonEdit != null)
//                {
//                    buttonEdit.ButtonPressed -= ButtonEdit_ButtonPressed;
//                }
//                var spinEdit = _activeEditor as RepositoryItemBaseSpinEdit;
//                if (spinEdit != null)
//                {
//                    spinEdit.Spin -= SpinEdit_Spin;
//                }
//                _activeEditor = null;
//            }
//        }
//        private void SpinEdit_Spin(object sender, SpinEventArgs e)
//        {
//            _mouseDownTime = 0;
//        }

//        private void gridView_EditorShowing(object sender, CancelEventArgs e)
//        {
//            _activeEditor = null;
//            RepositoryItem edit = GridView.FocusedColumn.ColumnEdit;
//            if (!e.Cancel)
//            {
//                if (edit != null)
//                {
//                    edit.MouseDown += Editor_MouseDown;
//                    edit.MouseUp += Editor_MouseUp;
//                    var buttonEdit = edit as RepositoryItemButtonEdit;
//                    if (buttonEdit != null)
//                    {
//                        buttonEdit.ButtonPressed += ButtonEdit_ButtonPressed;
//                    }
//                    var spinEdit = edit as RepositoryItemBaseSpinEdit;
//                    if (spinEdit != null)
//                    {
//                        spinEdit.Spin += SpinEdit_Spin;
//                    }
//                    edit.KeyDown += Editor_KeyDown;
//                    _activeEditor = edit;
//                }
//            }
//        }
//        private void ButtonEdit_ButtonPressed(object sender, ButtonPressedEventArgs e)
//        {
//            _mouseDownTime = 0;
//        }

        public override object FocusedObject {
            get {
                object result = null;
                if (GridView!= null) {
                    var focusedGridView = GetFocusedGridView(GridView);
                    result = GetFocusedRowObject(focusedGridView);
                    Window window = GridView.Window;
                    if (window != null)
                        result = GridView.Window.View.ObjectSpace.GetObject(result);
                }
                return result;
            }
            set {
                if ((value != null) && (GridView != null) && (DataSource != null))
                {
                    var focusedView = GridView;
                    XtraGridUtils.SelectRowByHandle(focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(focusedView)){
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(DevExpress.ExpressApp.Win.Editors.XafGridView view) {
            if (view is XafGridView&& ((XafGridView) view).Window== null)
                return XtraGridUtils.GetFocusedRowObject(view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return view.GetRow(rowHandle);
            return XtraGridUtils.GetFocusedRowObject(view);
        }

        DevExpress.ExpressApp.Win.Editors.XafGridView GetFocusedGridView(DevExpress.ExpressApp.Win.Editors.XafGridView view) {
            if (view != null) {
                Window masterWindow = ((XafGridView)view).MasterWindow;
                if (masterWindow != null) {
                    return (DevExpress.ExpressApp.Win.Editors.XafGridView)((GridListEditor)((ListView)masterWindow.View).Editor).Grid.FocusedView;
                }
            }
            if (GridView!= null)
                return (DevExpress.ExpressApp.Win.Editors.XafGridView)GridView.GridControl.FocusedView??GridView;
            return GridView;
        }

//        protected override void ProcessMouseClick(EventArgs e)
//        {
//            if (!_selectedItemExecuting) {
//                var view = GetFocusedGridView(GridView);
//                if (view.FocusedRowHandle >= 0)
//                {
//                    DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(Grid, e);
//                    GridHitInfo hitInfo = GridView.CalcHitInfo(args.Location);
//                    if (hitInfo.InRow && ((hitInfo.HitTest == GridHitTest.RowCell || (view is XafGridView&&((XafGridView) view).Window != null)))){
//                        args.Handled = true;
//                        OnProcessSelectedItem();
//                    }
//                }
//            }
//        }

//        protected override void OnProcessSelectedItem()
//        {
//            _selectedItemExecuting = true;
//            try
//            {
//                if ((GridView != null) && (GridView.ActiveEditor != null))
//                {
//                    BindingHelper.EndCurrentEdit(Grid);
//                }
//                base.OnProcessSelectedItem();
//            }
//            finally
//            {
//                _selectedItemExecuting = false;
//            }
//        }

        
        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
        }

        protected override GridControl CreateGridControl() {
            var customGridCreateEventArgs = new CustomGridCreateEventArgs();
            OnCustomGridCreate(customGridCreateEventArgs);
            return customGridCreateEventArgs.Handled ? customGridCreateEventArgs.Grid : base.CreateGridControl();
        }

        public GridListEditor(IModelListView model)
            : base(model)
        {
        }
        public GridListEditor() : this(null) { }

        #region IDXPopupMenuHolder Members
        bool IDXPopupMenuHolder.CanShowPopupMenu(Point position)
        {
            if (CanShowPopupMenu(position))
                return !_hidePopupMenu;
            return false;
        }
        #endregion

        protected override DevExpress.ExpressApp.Win.Editors.XafGridView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs();
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            DevExpress.ExpressApp.Win.Editors.XafGridView gridViewCore = gridViewCreatingEventArgs.Handled ? gridViewCreatingEventArgs.GridView : new XafGridView(this);
            return gridViewCore;
        }


        public event EventHandler<CustomGetSelectedObjectsArgs> CustomGetSelectedObjects;
        protected override ModelSynchronizer CreateModelSynchronizer()
        {
            return new GridListEditorSynchronizer(this, Model);
        }
        private void OnCustomGetSelectedObjects(CustomGetSelectedObjectsArgs e)
        {
            EventHandler<CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public override IList GetSelectedObjects()
        {
            if (Grid!=null&&GridView != null)
            {
                var selectedObjects = GetSelectedObjects(GetFocusedGridView(GridView));
//                var selectedObjects = base.GetSelectedObjects();
                var e = new CustomGetSelectedObjectsArgs(selectedObjects);
                OnCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return base.GetSelectedObjects();
        }
        IList GetSelectedObjects(GridView focusedView)
        {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0))
            {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(rowHandle
                                                                                                     => focusedView.GetRow(rowHandle)).Where(obj => obj != null);
                return objects.ToList();
            }
            return new List<object>();
        }

        #region IPopupMenuHider Members
        public bool HidePopupMenu
        {
            get { return _hidePopupMenu; }
            set { _hidePopupMenu = value; }
        }

        
        #endregion
    }

    public class CustomGridCreateEventArgs:HandledEventArgs {
        public GridControl Grid { get; set; }
    }

    public class GridListEditorSynchronizer : DevExpress.ExpressApp.Win.Editors.GridListEditorSynchronizer
    {
        public GridListEditorSynchronizer(DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.GridView, model));
            foreach (var modelColumn in model.Columns) {
                Add(new ColumnOptionsModelSynchronizer(gridListEditor.GridView, modelColumn));
            }
        }
    }

    public class GridViewInstanceCreatedArgs : EventArgs {
        readonly XafGridView _xafGridView;

        public GridViewInstanceCreatedArgs(XafGridView xafGridView) {
            _xafGridView = xafGridView;
        }

        public XafGridView XafGridView {
            get { return _xafGridView; }
        }
    }


    public class CustomGridViewCreateEventArgs : HandledEventArgs
    {
        public XafGridView GridView { get; set; }
    }



    public class CustomGetSelectedObjectsArgs:HandledEventArgs
    {
        public CustomGetSelectedObjectsArgs(IList list)
        {
            List = list;
        }

        public IList List { get; set; }
    }
}