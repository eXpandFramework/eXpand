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
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Xpand.ExpressApp.ListEditors;

namespace Xpand.ExpressApp.Win.ListEditors {
    [ListEditor(typeof(object))]
    public class XpandGridListEditor : GridListEditor, IDXPopupMenuHolder, IPopupMenuHider {
        private bool _hidePopupMenu;

        public new XpandXafGridView GridView {
            get { return (XpandXafGridView)base.GridView; }
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
        protected override IModelSynchronizable CreateModelSynchronizer() {
            return new GridListEditorSynchronizer(this, Model);
        }

        protected override XafGridView CreateGridViewCore() {
            return new XpandXafGridView(this);
        }

        #region IPopupMenuHider Members
        public bool HidePopupMenu {
            get { return _hidePopupMenu; }
            set { _hidePopupMenu = value; }
        }


        #endregion

    }


    public class GridListEditorSynchronizer : DevExpress.ExpressApp.Win.Editors.GridListEditorSynchronizer {
        private readonly ModelSynchronizerList modelSynchronizerList;
        public GridListEditorSynchronizer(GridListEditor gridListEditor, IModelListView model)
            : base(gridListEditor, model) {
            modelSynchronizerList = new ModelSynchronizerList { new GridViewOptionsModelSynchronizer(gridListEditor.GridView, model) };
            foreach (var modelColumn in model.Columns) {
                modelSynchronizerList.Add(new ColumnOptionsModelSynchronizer(gridListEditor.GridView, modelColumn));
            }
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            modelSynchronizerList.ApplyModel();
        }

        public override void SynchronizeModel() {
            base.SynchronizeModel();
            modelSynchronizerList.SynchronizeModel();
        }
    }




    public class CustomGetSelectedObjectsArgs : HandledEventArgs {
        public CustomGetSelectedObjectsArgs(IList list) {
            List = list;
        }

        public IList List { get; set; }
    }
}