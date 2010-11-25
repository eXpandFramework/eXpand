using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.Win.ListEditors {
    [ListEditor(typeof(object))]
    public class XpandGridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider {
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
        public new XpandXafGridView GridView {
            get { return (XpandXafGridView)base.GridView; }
        }


        public override object FocusedObject {
            get {
                object result = null;
                if (GridView != null) {
                    var focusedGridView = GetFocusedGridView(GridView);
                    result = GetFocusedRowObject(focusedGridView);
                    Window window = GridView.Window;
                    if (window != null)
                        result = GridView.Window.View.ObjectSpace.GetObject(result);
                }
                return result;
            }
            set {
                if ((value != null) && (GridView != null) && (DataSource != null)) {
                    var focusedView = GridView;
                    XtraGridUtils.SelectRowByHandle(focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(focusedView)) {
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(DevExpress.ExpressApp.Win.Editors.XafGridView view) {
            if (view is XpandXafGridView && ((XpandXafGridView)view).Window == null)
                return XtraGridUtils.GetFocusedRowObject(view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return view.GetRow(rowHandle);
            return XtraGridUtils.GetFocusedRowObject(view);
        }

        DevExpress.ExpressApp.Win.Editors.XafGridView GetFocusedGridView(DevExpress.ExpressApp.Win.Editors.XafGridView view) {
            Frame masterFrame = ((XpandXafGridView)view).MasterFrame;
            if (masterFrame != null) {
                return (DevExpress.ExpressApp.Win.Editors.XafGridView)((XpandGridListEditor)((XpandListView)masterFrame.View).Editor).Grid.FocusedView;
            }
            return view;
        }




        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
        }


        public XpandGridListEditor(IModelListView model)
            : base(model) {
        }
        public XpandGridListEditor() : this(null) { }

        #region IDXPopupMenuHolder Members
        bool IDXPopupMenuHolder.CanShowPopupMenu(Point position) {
            if (CanShowPopupMenu(position))
                return !_hidePopupMenu;
            return false;
        }
        #endregion

        protected override DevExpress.ExpressApp.Win.Editors.XafGridView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs();
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            DevExpress.ExpressApp.Win.Editors.XafGridView gridViewCore = gridViewCreatingEventArgs.Handled ? gridViewCreatingEventArgs.GridView : new XpandXafGridView(this);
            return gridViewCore;
        }


        public event EventHandler<CustomGetSelectedObjectsArgs> CustomGetSelectedObjects;
        protected override ModelSynchronizer CreateModelSynchronizer() {
            return new GridListEditorSynchronizer(this, Model);
        }
        private void OnCustomGetSelectedObjects(CustomGetSelectedObjectsArgs e) {
            EventHandler<CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public override IList GetSelectedObjects() {
            if (Grid != null && GridView != null) {
                var selectedObjects = GetSelectedObjects(GetFocusedGridView(GridView));
                var e = new CustomGetSelectedObjectsArgs(selectedObjects);
                OnCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return base.GetSelectedObjects();
        }
        IList GetSelectedObjects(GridView focusedView) {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0)) {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(rowHandle
                                                                                                     => focusedView.GetRow(rowHandle)).Where(obj => obj != null);
                return objects.ToList();
            }
            return new List<object>();
        }

        protected override void ProcessMouseClick(EventArgs e) {
            var view = Grid.FocusedView as XpandXafGridView;
            if (view.FocusedRowHandle >= 0) {
                DXMouseEventArgs mouseArgs = DXMouseEventArgs.GetMouseArgs(this.Grid, e);
                GridHitInfo info = GridView.CalcHitInfo(mouseArgs.Location);
                if (info.InRow && (info.HitTest == GridHitTest.RowDetail)) {
                    mouseArgs.Handled = true;
                    var showViewParameter = new ShowViewParameters();
                    ListViewProcessCurrentObjectController.ShowObject(view.GetRow(view.FocusedRowHandle), showViewParameter, view.MasterFrame.Application, view.Window, view.Window.View);
                    view.MasterFrame.Application.ShowViewStrategy.ShowView(showViewParameter, new ShowViewSource(null, null));
                    return;
                }
            }

            base.ProcessMouseClick(e);
        }

        #region IPopupMenuHider Members
        public bool HidePopupMenu {
            get { return _hidePopupMenu; }
            set { _hidePopupMenu = value; }
        }


        #endregion
    }

    public class CustomGridCreateEventArgs : HandledEventArgs {
        public GridControl Grid { get; set; }
    }

    public class GridListEditorSynchronizer : DevExpress.ExpressApp.Win.Editors.GridListEditorSynchronizer {
        public GridListEditorSynchronizer(DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor, IModelListView model)
            : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.GridView, model));
            foreach (var modelColumn in model.Columns) {
                Add(new ColumnOptionsModelSynchronizer(gridListEditor.GridView, modelColumn));
            }
        }
    }

    public class GridViewInstanceCreatedArgs : EventArgs {
        readonly XpandXafGridView _xpandXafGridView;

        public GridViewInstanceCreatedArgs(XpandXafGridView xpandXafGridView) {
            _xpandXafGridView = xpandXafGridView;
        }

        public XpandXafGridView XpandXafGridView {
            get { return _xpandXafGridView; }
        }
    }


    public class CustomGridViewCreateEventArgs : HandledEventArgs {
        public XpandXafGridView GridView { get; set; }
    }



    public class CustomGetSelectedObjectsArgs : HandledEventArgs {
        public CustomGetSelectedObjectsArgs(IList list) {
            List = list;
        }

        public IList List { get; set; }
    }
}