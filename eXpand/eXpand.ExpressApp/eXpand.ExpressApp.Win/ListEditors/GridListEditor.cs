using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Win.ListEditors
{
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