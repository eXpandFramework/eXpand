using System;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView;
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

        DevExpress.XtraGrid.Views.Base.ColumnView IColumnViewEditor.ColumnView {
            get { return GridView; }
        }
        public new XpandXafLayoutView GridView {
            get { return Grid != null ? (XpandXafLayoutView)Grid.MainView : null; }
        }

        CollectionSourceBase IColumnViewEditor.CollectionSource {
            get { return CollectionSource; }
        }

        protected virtual void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;

        bool IColumnViewEditor.IsAsyncServerMode() {
            var source = CollectionSource as CollectionSource;
            return ((source != null) && source.IsServerMode && source.IsAsyncServerMode);
        }

        protected override XafLayoutViewColumn CreateColumn() {
            return new XpandXafLayoutColumn(ObjectTypeInfo, this);
        }

        protected override IModelSynchronizable CreateModelSynchronizer() {
            var modelSynchronizable = base.CreateModelSynchronizer();
            var synchronizer = new LayoutViewLstEditorDynamicModelSynchronizer(this);
            synchronizer.ModelSynchronizerList.Insert(0, modelSynchronizable);
            return synchronizer;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        bool IColumnViewEditor.OverrideViewDesignMode { get; set; }

        protected override void OnColumnCreated(GridColumn column, IModelColumn columnInfo) {
            base.OnColumnCreated(column, columnInfo);
            if (column.ColumnEdit is RepositoryItemMemoExEdit)
                column.ColumnEdit = new RepositoryItemMemoEdit { Name = columnInfo.PropertyName };
        }

        protected override XafLayoutView CreateLayoutViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return (XafLayoutView)(gridViewCreatingEventArgs.Handled
                     ? gridViewCreatingEventArgs.GridView
                     : new XpandXafLayoutView(this) { OverrideViewDesignMode = ((IColumnViewEditor)this).OverrideViewDesignMode });
        }

        bool ISupportFooter.IsFooterVisible {
            get { return false; }
            set { }
        }
    }

    public class XpandXafLayoutView : XafLayoutView, IMasterDetailColumnView {
        readonly LayoutViewListEditor _gridListEditor;

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
        public bool CanFilterGroupSummaryColumns { get; set; }

        BaseView IMasterDetailColumnView.GetDetailView(int rowHandle, int relationIndex) {
            throw new NotImplementedException();
        }

        string IMasterDetailColumnView.GetRelationName(int rowHandle, int relationIndex) {
            throw new NotImplementedException();
        }

        event MasterRowGetRelationCountEventHandler IMasterDetailColumnView.MasterRowGetRelationCount {
            add { throw new NotImplementedException(); }
            remove { }
        }

        event MasterRowGetRelationNameEventHandler IMasterDetailColumnView.MasterRowGetRelationName {
            add { throw new NotImplementedException(); }
            remove { }
        }

        event MasterRowGetRelationNameEventHandler IMasterDetailColumnView.MasterRowGetRelationDisplayCaption {
            add { throw new NotImplementedException(); }
            remove { }
        }

        event MasterRowGetChildListEventHandler IMasterDetailColumnView.MasterRowGetChildList {
            add { throw new NotImplementedException(); }
            remove { }
        }

        event MasterRowEmptyEventHandler IMasterDetailColumnView.MasterRowEmpty {
            add { throw new NotImplementedException(); }
            remove { }
        }

        event MasterRowGetLevelDefaultViewEventHandler IMasterDetailColumnView.MasterRowGetLevelDefaultView {
            add { throw new NotImplementedException(); }
            remove { }
        }

        int IMasterDetailColumnView.GetRelationIndex(int sourceRowHandle, string levelName) {
            throw new NotImplementedException();
        }
        #endregion
        public XpandXafLayoutView(LayoutViewListEditor layoutViewListEditor)
            : base(layoutViewListEditor.Grid) {
            _gridListEditor = layoutViewListEditor;
        }

        protected override bool IsDesignMode {
            get {
                return OverrideViewDesignMode || base.IsDesignMode;
            }
        }

        protected override void AssignColumns(DevExpress.XtraGrid.Views.Base.ColumnView cv, bool synchronize) {
            if (_gridListEditor == null) {
                base.AssignColumns(cv, synchronize);
                return;
            }
            if (synchronize) {
                base.AssignColumns(cv, true);
            } else {
                Columns.Clear();
                var gridColumns = _gridListEditor.GridView.Columns.OfType<IXafGridColumn>().ToList();
                foreach (var column in gridColumns) {
                    var xpandXafGridColumn = column.CreateNew(column.TypeInfo, _gridListEditor);
                    xpandXafGridColumn.ApplyModel(column.Model);
                    Columns.Add((GridColumn)xpandXafGridColumn);
                    xpandXafGridColumn.Assign((GridColumn)column);
                }
            }
        }

        protected override BaseView CreateInstance() {
            return new XpandXafLayoutView(GridControl);
        }
        public bool OverrideViewDesignMode { get; set; }
    }
    public class XpandXafLayoutColumn : XafLayoutViewColumn, IXafGridColumn {
        public XpandXafLayoutColumn(ITypeInfo typeInfo, LayoutViewListEditorBase listEditor)
            : base(typeInfo, listEditor) {
        }

        public override int VisibleIndex {
            get {
                var index = Model.Index;
                return index.HasValue ? index.Value : 0;
            }
            set {
                base.VisibleIndex = value;
            }
        }
        #region Implementation of IXafGridColumn
        public new void Assign(GridColumn gridColumn) {
            base.Assign(gridColumn);
        }

        public bool AllowSummaryChange {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        public ColumnsListEditor Editor {
            get { return ListEditor; }
        }

        public IXafGridColumn CreateNew(ITypeInfo typeInfo, ColumnsListEditor editor) {
            return new XpandXafLayoutColumn(typeInfo, (LayoutViewListEditorBase)editor);
        }
        #endregion
    }

}
