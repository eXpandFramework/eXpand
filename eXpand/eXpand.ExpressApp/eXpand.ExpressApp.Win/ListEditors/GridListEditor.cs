using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using eXpand.ExpressApp.ListEditors;
using System.Linq;

namespace eXpand.ExpressApp.Win.ListEditors
{
    public class GridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider
    {
        private bool hidePopupMenu;
        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;

        public void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
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
            var selectedObjects = base.GetSelectedObjects();
            var e = new CustomGetSelectedObjectsArgs(selectedObjects);
            InvokeCustomGetSelectedObjects(e);
            if (e.Handled)
                return e.List;
            return selectedObjects;
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



        internal Window Window { get; set; }
        internal string OwnerPropertyName { get; set; }


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
            OwnerPropertyName= xafGridView.OwnerPropertyName;
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
                var gridColumns = _gridListEditor.GridView.Columns.OfType<DevExpress.ExpressApp.Win.Editors.XafGridColumn>().Where(column => column.PropertyName!=OwnerPropertyName);
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