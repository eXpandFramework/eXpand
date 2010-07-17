using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using eXpand.ExpressApp.ListEditors;
using System.Linq;

namespace eXpand.ExpressApp.Win.ListEditors
{
    public class GridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider
    {
        private bool hidePopupMenu;
        bool _selectedItemExecuting;
        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;

        public void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }
        public new XafGridView GridView
        {
            get { return (XafGridView) GetGridView(base.GridView); }
        }

        public override object FocusedObject {
            get {
                object result = null;
                if (GridView!= null) {
                    result = GetFocusedRowObject(GridView);
                }
                return result;
            }
            set {
                if ((value != null) && (GetGridView(GridView) != null) && (DataSource != null))
                {
                    var focusedView = GridView;
                    XtraGridUtils.SelectRowByHandle(focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(focusedView)){
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(XafGridView view) {
            if (view.Window== null)
                return XtraGridUtils.GetFocusedRowObject(view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return view.GetRow(rowHandle);
            return XtraGridUtils.GetFocusedRowObject(view);
        }

        GridView GetGridView(DevExpress.ExpressApp.Win.Editors.XafGridView view) {
            if (view != null) {
                Window masterWindow = ((XafGridView)view).MasterWindow;
                if (masterWindow != null) {
                    return (GridView)((GridListEditor)((ListView)masterWindow.View).Editor).Grid.FocusedView;
                }
            }
            if (base.GridView!= null)
                return (GridView)base.GridView.GridControl.FocusedView;
            return null;
        }

        protected override void ProcessMouseClick(EventArgs e)
        {
            if (!_selectedItemExecuting) {
                var view = GridView;
                if (view.FocusedRowHandle >= 0)
                {
                    DXMouseEventArgs args = DXMouseEventArgs.GetMouseArgs(Grid, e);
                    GridHitInfo hitInfo = GridView.CalcHitInfo(args.Location);
                    if (hitInfo.InRow && ((hitInfo.HitTest == GridHitTest.RowCell || (view.Window != null)))){
                        args.Handled = true;
                        OnProcessSelectedItem();
                    }
                }
            }
        }

        protected override void OnProcessSelectedItem()
        {
            _selectedItemExecuting = true;
            try
            {
                if ((GridView != null) && (GridView.ActiveEditor != null))
                {
                    BindingHelper.EndCurrentEdit(Grid);
                }
                base.OnProcessSelectedItem();
            }
            finally
            {
                _selectedItemExecuting = false;
            }
        }

        
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
                return !hidePopupMenu;
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
        private void InvokeCustomGetSelectedObjects(CustomGetSelectedObjectsArgs e)
        {
            EventHandler<CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public override IList GetSelectedObjects()
        {
            if (Grid!=null&&GridView != null)
            {
                var selectedObjects = GetSelectedObjects(GridView);
                var e = new CustomGetSelectedObjectsArgs(selectedObjects);
                InvokeCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return new List<object>();
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
            get { return hidePopupMenu; }
            set { hidePopupMenu = value; }
        }

        
        #endregion
    }

    public class CustomGridCreateEventArgs:HandledEventArgs {
        public GridControl Grid { get; set; }
    }

    public class GridListEditorSynchronizer : DevExpress.ExpressApp.Win.Editors.GridListEditorSynchronizer
    {
        public GridListEditorSynchronizer(DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.GridView,model));
        }
    }

    public class XafGridView : DevExpress.ExpressApp.Win.Editors.XafGridView
    {
        readonly DevExpress.ExpressApp.Win.Editors.GridListEditor _gridListEditor;
        private static readonly object instanceCreated = new object();

        public XafGridView(DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor) {
            _gridListEditor = gridListEditor;
        }
        protected virtual void OnInstanceCreated(GridViewInstanceCreatedArgs e)
        {
            var handler = (EventHandler<GridViewInstanceCreatedArgs>)Events[instanceCreated];
            if (handler != null) handler(this, e);
        }

        [Description("Provides the ability to customize cell merging behavior."), Category("Merge")]
        public event EventHandler<GridViewInstanceCreatedArgs> GridViewInstanceCreated
        {
            add { Events.AddHandler(instanceCreated, value); }
            remove { Events.RemoveHandler(instanceCreated, value); }
        }


        public Window Window { get; set; }

        public Window MasterWindow { get; set; }

//        internal string OwnerPropertyName { get; set; }


        protected override BaseView CreateInstance()
        {
            var view = new XafGridView(_gridListEditor);
            view.SetGridControl(GridControl);
            OnInstanceCreated(new GridViewInstanceCreatedArgs(view));
            return view;
        }
        public override void Assign(BaseView v, bool copyEvents)
        {
            var xafGridView = ((XafGridView) v);
//            OwnerPropertyName= xafGridView.OwnerPropertyName;
            Window = xafGridView.Window;
            Events.AddHandler(instanceCreated, xafGridView.Events[instanceCreated]);
            base.Assign(v, copyEvents);
        }
        protected override void AssignColumns(ColumnView cv, bool synchronize) {
            if (synchronize) {
                base.AssignColumns(cv, true);
            }
            else {
                Columns.Clear();
                var columnsListEditorModelSynchronizer = new ColumnsListEditorModelSynchronizer(_gridListEditor,_gridListEditor.Model);
                columnsListEditorModelSynchronizer.ApplyModel();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<DevExpress.ExpressApp.Win.Editors.XafGridColumn>();
                foreach (var column in gridColumns) {
                    Columns.Add(column);
                }
                
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

    public class XafGridColumn : DevExpress.ExpressApp.Win.Editors.XafGridColumn
    {
        readonly GridListEditor _gridListEditor;
        public XafGridColumn(ITypeInfo typeInfo, DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor) : base(typeInfo, gridListEditor) {
            _gridListEditor = (GridListEditor) gridListEditor;
        }
        protected internal new virtual void Assign(GridColumn column) {
            base.Assign(column);
        }

        public GridListEditor GridListEditor {
            get { return _gridListEditor; }
        }
    }
    public class CustomAssignColumnsArgs : HandledEventArgs {
        readonly ColumnView _columnView;
        readonly bool _synchronize;

        public CustomAssignColumnsArgs(ColumnView columnView, bool synchronize) {
            _columnView = columnView;
            _synchronize = synchronize;
        }

        public ColumnView ColumnView {
            get { return _columnView; }
        }

        public bool Synchronize {
            get { return _synchronize; }
        }
    }

    public class CustomGridViewCreateEventArgs : HandledEventArgs
    {
        public XafGridView GridView { get; set; }
    }


    public class InstanceCreatedArgs : EventArgs {
        readonly BaseView _baseView;

        public InstanceCreatedArgs(BaseView baseView) {
            _baseView = baseView;
        }

        public BaseView BaseView {
            get { return _baseView; }
        }
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