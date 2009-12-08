using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Controls;

namespace eXpand.ExpressApp.Win.ListEditors
{
    public class GridListEditor : DevExpress.ExpressApp.Win.Editors.GridListEditor, IDXPopupMenuHolder, IPopupMenuHider
    {
//        private readonly CultureInfo culture;
        private bool hidePopupMenu;

        public GridListEditor(DictionaryNode info) : base(info)
        {
//            culture = GetCulture(info);
        }

        public GridListEditor()
        {
        }
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
//        public static CultureInfo GetCulture(DictionaryNode info)
//        {
//            var culture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
//            culture.NumberFormat.NumberDecimalSeparator = info.GetRootNode().GetAttributeValue("NumberDecimalSeparator");
//            culture.NumberFormat.NumberGroupSeparator = info.GetRootNode().GetAttributeValue("NumberGroupSeparator");
//            return culture;
//        }

//        protected override void OnColumnCreated(GridColumn column, ColumnInfoNodeWrapper columnInfo)
//        {
//            base.OnColumnCreated(column, columnInfo);
//            var repositoryItemIntegerEdit = column.RealColumnEdit as RepositoryItemSpinEdit;
//            if (repositoryItemIntegerEdit != null)
//                repositoryItemIntegerEdit.Mask.Culture = culture;
//        }
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