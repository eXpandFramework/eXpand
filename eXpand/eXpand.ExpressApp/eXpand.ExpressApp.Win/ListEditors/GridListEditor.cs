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
        private bool _hidePopupMenu;
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
//            if (view != null) {
                Window masterWindow = ((XafGridView)view).MasterWindow;
                if (masterWindow != null) {
                    return (DevExpress.ExpressApp.Win.Editors.XafGridView)((GridListEditor)((ListView)masterWindow.View).Editor).Grid.FocusedView;
                }
                return view;
//            }
//            return view;
//            if (GridView!= null)
//                return (DevExpress.ExpressApp.Win.Editors.XafGridView)GridView.GridControl.FocusedView??GridView;
//            return GridView;
        }



        
        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
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