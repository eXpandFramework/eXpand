using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView.Model;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.LayoutView {
    [ListEditor(typeof(object), false)]
    public class LayoutViewListEditor : LayoutViewListEditorBase, IColumnViewEditor {
        public LayoutViewListEditor(IModelListView model)
            : base(model) {
        }
        public new IModelListViewOptionsLayoutView Model {
            get { return (IModelListViewOptionsLayoutView)base.Model; }
        }

        protected virtual void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;
        protected override List<IModelSynchronizable> CreateModelSynchronizers() {
            List<IModelSynchronizable> result = base.CreateModelSynchronizers();
            result.Add(new FilterModelSynchronizer(this, Model));
            result.Add(new LayoutViewListEditorSynchronizer(this));
            result.Add(new LayoutViewOptionsSynchronizer(this));
            result.Add(new LayoutColumnOptionsSynchroniser(this));
            result.Add(new RepositoryItemColumnViewSynchronizer(ColumnView, Model));
            result.Add(new LayoutViewLayoutStoreSynchronizer(this));
            return result;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool IColumnViewEditor.OverrideViewDesignMode { get; set; }

        protected override void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
            base.OnColumnCreated(column, columnInfo);
            if (column.ColumnEdit is RepositoryItemMemoExEdit)
                column.ColumnEdit = new RepositoryItemMemoEdit { Name = columnInfo.PropertyName };
        }
        protected override DevExpress.XtraGrid.Views.Base.ColumnView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return gridViewCreatingEventArgs.Handled
                ? gridViewCreatingEventArgs.GridView
                : new XpandXafLayoutView(this) { OverrideViewDesignMode = ((IColumnViewEditor)this).OverrideViewDesignMode };
        }

        bool ISupportFooter.IsFooterVisible {
            get { return false; }
            set { }
        }

    }

    public class XpandXafLayoutView : XafLayoutView, IMasterDetailColumnView {

        public XpandXafLayoutView(GridControl gridControl)
            : base(gridControl) {
        }
        public override void Assign(BaseView v, bool copyEvents) {
            var xafGridView = (IMasterDetailColumnView)v;
            ((IMasterDetailColumnView)this).Window = xafGridView.Window;
            ((IMasterDetailColumnView)this).MasterFrame = xafGridView.MasterFrame;
            base.Assign(v, copyEvents);
        }
        #region Implementation of IMasterDetailColumnView
        public Window Window { get; set; }
        public Frame MasterFrame { get; set; }

        int IMasterDetailColumnView.GetRelationIndex(int sourceRowHandle, string levelName){
            throw new NotImplementedException();
        }

        public bool CanFilterGroupSummaryColumns { get; set; }
        #endregion
        public XpandXafLayoutView(LayoutViewListEditor layoutViewListEditor)
            : base(layoutViewListEditor.Grid) {
        }

        protected override bool IsDesignMode {
            get {
                return OverrideViewDesignMode || base.IsDesignMode;
            }
        }
        protected override BaseView CreateInstance() {
            return new XpandXafLayoutView(GridControl);
        }
        public bool OverrideViewDesignMode { get; set; }
    }

}
