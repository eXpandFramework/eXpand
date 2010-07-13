using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Model;
using eXpand.ExpressApp.ListEditors;

namespace eXpand.ExpressApp.Win.ListEditors
{
    public class GridListEditorSynchronizer : DevExpress.ExpressApp.Win.Editors.GridListEditorSynchronizer
    {
        public GridListEditorSynchronizer(DevExpress.ExpressApp.Win.Editors.GridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.GridView,model));
        }
    }
    public class GridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider
    {
        private bool hidePopupMenu;

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

    public class CustomGetSelectedObjectsArgs:HandledEventArgs
    {
        public CustomGetSelectedObjectsArgs(IList list)
        {
            List = list;
        }

        public IList List { get; set; }
    }
}