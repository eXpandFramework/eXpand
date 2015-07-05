using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.FilterEditor;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Fasterflect;
using Xpand.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;
using Xpand.Persistent.Base.General.Model.Options;
using ListView = DevExpress.ExpressApp.ListView;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView {
    [ListEditor(typeof(object), false)]
    public class XpandGridListEditor : GridListEditor, IColumnViewEditor, IDXPopupMenuHolder {
        public XpandGridListEditor(IModelListView model)
            : base(model) {
        }
        public event EventHandler<CustomGridViewCreateEventArgs> CustomGridViewCreate;

        bool IColumnViewEditor.OverrideViewDesignMode { get; set; }

        public new IModelListViewOptionsGridView Model {
            get { return (IModelListViewOptionsGridView)base.Model; }
        }

        protected override List<IModelSynchronizable> CreateModelSynchronizers() {
            var listEditorSynchronizer = new XpandGridListEditorSynchronizer(this);
            var dynamicModelSynchronizer = new GridViewListEditorDynamicModelSynchronizer(GridView,Model,((IColumnViewEditor)this).OverrideViewDesignMode);
            dynamicModelSynchronizer.ModelSynchronizerList.Insert(0, listEditorSynchronizer);
            return dynamicModelSynchronizer.ModelSynchronizerList;
        }

        #region modelDetailViews
        private void OnCustomGetSelectedObjects(CustomGetSelectedObjectsArgs e) {
            EventHandler<CustomGetSelectedObjectsArgs> customGetSelectedObjectsHandler = CustomGetSelectedObjects;
            if (customGetSelectedObjectsHandler != null) customGetSelectedObjectsHandler(this, e);
        }

        public event EventHandler<CustomGridCreateEventArgs> CustomGridCreate;

        protected virtual void OnCustomGridViewCreate(CustomGridViewCreateEventArgs e) {
            EventHandler<CustomGridViewCreateEventArgs> handler = CustomGridViewCreate;
            if (handler != null) handler(this, e);
        }

        public override object FocusedObject {
            get {
                object result = null;
                if (GridView != null) {
                    var focusedGridView = GetFocusedGridView(GridView);
                    result = GetFocusedRowObject(focusedGridView);
                    var masterDetailXafGridView = ((IMasterDetailColumnView) GridView);
                    Window window = masterDetailXafGridView.Window;
                    if (window != null)
                        result = masterDetailXafGridView.Window.View.ObjectSpace.GetObject(result);
                }
                return result;
            }
            set {
                if (value != null && value != DBNull.Value && GridView != null && DataSource != null) {
                    var focusedView = GridView;
                    XtraGridUtils.SelectRowByHandle(focusedView, focusedView.GetRowHandle(List.IndexOf(value)));
                    if (XtraGridUtils.HasValidRowHandle(focusedView)) {
                        focusedView.SetRowExpanded(focusedView.FocusedRowHandle, true, true);
                    }
                }
            }
        }

        object GetFocusedRowObject(DevExpress.XtraGrid.Views.Base.ColumnView view) {
            if (((IMasterDetailColumnView) view).Window == null)
                return XtraGridUtils.GetFocusedRowObject(CollectionSource, view);
            int rowHandle = view.FocusedRowHandle;
            if (!((!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))))
                return XtraGridUtils.GetFocusedRowObject(view);
            return XtraGridUtils.GetFocusedRowObject(CollectionSource, view);
        }

        DevExpress.XtraGrid.Views.Base.ColumnView GetFocusedGridView(DevExpress.XtraGrid.Views.Base.ColumnView view) {
            Frame masterFrame = ((IMasterDetailColumnView) view).MasterFrame;
            return masterFrame != null && masterFrame.View != null ? GetFocusedGridView(masterFrame) : view;
        }

        DevExpress.XtraGrid.Views.Base.ColumnView GetFocusedGridView(Frame masterFrame) {
            return (DevExpress.XtraGrid.Views.Base.ColumnView)((WinColumnsListEditor)((ListView)masterFrame.View).Editor).Grid.FocusedView;
        }

        public void OnCustomGridCreate(CustomGridCreateEventArgs e) {
            EventHandler<CustomGridCreateEventArgs> handler = CustomGridCreate;
            if (handler != null) handler(this, e);
        }


        protected override DevExpress.XtraGrid.Views.Base.ColumnView CreateGridViewCore() {
            var gridViewCreatingEventArgs = new CustomGridViewCreateEventArgs(Grid);
            OnCustomGridViewCreate(gridViewCreatingEventArgs);
            return gridViewCreatingEventArgs.Handled ? gridViewCreatingEventArgs.GridView : CreateXpandGridView();
        }

        private DevExpress.XtraGrid.Views.Grid.GridView CreateXpandGridView(){
            if (CanShowBands){
                var gridView = new XpandBandedGridView();
                gridView.OptionsView.ColumnAutoWidth = true;
                gridView.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
                return gridView;
            }
            return new XpandXafGridView();
        }

        public event EventHandler<CustomGetSelectedObjectsArgs> CustomGetSelectedObjects;
        public override IList GetSelectedObjects() {
            if (Grid != null && GridView != null) {
                var focusedGridView = GetFocusedGridView(GridView);
                var selectedObjects = GetSelectedObjects(focusedGridView);
                var e = new CustomGetSelectedObjectsArgs(selectedObjects);
                OnCustomGetSelectedObjects(e);
                if (e.Handled)
                    return e.List;
                return selectedObjects;
            }
            return base.GetSelectedObjects();
        }
        IList GetSelectedObjects(DevExpress.XtraGrid.Views.Base.ColumnView focusedView) {
            int[] selectedRows = focusedView.GetSelectedRows();
            if ((selectedRows != null) && (selectedRows.Length > 0)) {
                IEnumerable<object> objects = selectedRows.Where(rowHandle => rowHandle > -1).Select(focusedView.GetRow).Where(obj => obj != null);
                return objects.ToList();
            }
            return new List<object>();
        }

        #endregion
        bool IDXPopupMenuHolder.CanShowPopupMenu(Point position) {
            var focusedView = Grid.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (focusedView != null){
                var hitTest = focusedView.CalcHitInfo(Grid.PointToClient(position)).HitTest;
                return ((hitTest == GridHitTest.Row)
                        || (hitTest == GridHitTest.RowCell)
                        || (hitTest == GridHitTest.EmptyRow)
                        || (hitTest == GridHitTest.None));
            }
            return false;
        }
    }

    public class XpandFilterBuilder : FilterBuilder {
        private readonly IEnumerable<IModelMember> _modelMembers;

        public XpandFilterBuilder(FilterColumnCollection columns, IDXMenuManager manager, UserLookAndFeel lookAndFeel, DevExpress.XtraGrid.Views.Base.ColumnView view, FilterColumn fColumn, IEnumerable<IModelMember> modelMembers): base(columns, manager, lookAndFeel, view, fColumn){
            _modelMembers = modelMembers;
        }

        protected override void OnFilterControlCreated(IFilterControl filterControl){
            base.OnFilterControlCreated(filterControl);
            var view = (DevExpress.XtraGrid.Views.Base.ColumnView) this.GetFieldValue("view");
            fcMain = new XpandGridFilterControl(() => view.ActiveFilterCriteria, () => _modelMembers) {
                UseMenuForOperandsAndOperators = view.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators,
                AllowAggregateEditing = view.OptionsFilter.FilterEditorAggregateEditing,
            };
        }
    }

    public class XpandBandedGridView:XafBandedGridView,IMasterDetailColumnView{
        public Window Window { get; set; }

        public Frame MasterFrame { get; set; }

        public override void Assign(BaseView baseView, bool copyEvents) {
            var xafGridView = ((IMasterDetailColumnView)baseView);
            xafGridView.AssignMasterDetail(this);
            base.Assign(baseView, copyEvents);
        }

        protected override XafBandedGridView CreateInstanceCore(){
            return new XpandBandedGridView();
        }
    }

    public class XpandXafGridView : XafGridView, IMasterDetailColumnView {
        protected override XafGridView CreateInstanceCore() {
            return new XpandXafGridView();
        }
        #region modelDetailViews
        Window IMasterDetailColumnView.Window { get; set; }
        Frame IMasterDetailColumnView.MasterFrame { get; set; }

        public override void Assign(BaseView baseView, bool copyEvents) {
            var xafGridView = ((IMasterDetailColumnView)baseView);
            xafGridView.AssignMasterDetail(this);
            base.Assign(baseView, copyEvents);
        }
        #endregion
    }


}
