using System.Linq;
using DevExpress.Data.Summary;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.MasterDetail;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;
using GridViewModelSynchronizer = Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model.GridViewModelSynchronizer;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    public class GridViewListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public GridViewListEditorDynamicModelSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView) {
            ModelSynchronizerList.Add(new GridViewViewOptionsSynchronizer(gridView, modelListView, overrideViewDesignMode));
            ModelSynchronizerList.Add(new GridViewColumnOptionsSynchroniser(gridView, modelListView));
            ModelSynchronizerList.Add(new XpandGridSummaryModelSynchronizer(gridView, modelListView));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(gridView, modelListView));
        }
    }

    public class GridViewViewOptionsSynchronizer : ComponentSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelOptionsGridView> {
        public GridViewViewOptionsSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView.GridViewOptions, overrideViewDesignMode) {
        }
    }

    public class GridViewColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelListViewOptionsGridView, IModelColumnOptionsGridView> {
        public GridViewColumnOptionsSynchroniser(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelListView)
            : base(gridView, modelListView) {
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return Control;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsGridView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnGridView;
        }
    }

    public class XpandGridListEditorSynchronizer : ListEditorModelSynchronizer {
        public XpandGridListEditorSynchronizer(XpandGridListEditor gridListEditor)
            : base(gridListEditor) {
            ModelSynchronizerList.Add(new XpandGridViewModelSynchronizer(gridListEditor));
        }
    }

    public class XpandGridViewModelSynchronizer : GridViewModelSynchronizer {
        public XpandGridViewModelSynchronizer(XpandGridListEditor columnViewEditor)
            : base(columnViewEditor) {
        }
    }
    public class XpandGridSummaryModelSynchronizer : GridSummaryModelSynchronizer {
        public XpandGridSummaryModelSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView,IModelListView modelListView)
            : base(gridView,modelListView ) {
        }

        protected override void RemoveGroupSummaryForProtectedColumns() {
            for (int i = Control.GroupSummary.Count - 1; i >= 0; i--) {
                ISummaryItem item = Control.GroupSummary[i];
                foreach (GridColumn column in Control.Columns) {
                    var xafGridColumn = column as XafGridColumn;
                    if ((xafGridColumn != null) && xafGridColumn.FieldName == item.FieldName && !new XafGridColumnWrapper(xafGridColumn).AllowGroupingChange) {
                        Control.GroupSummary.RemoveAt(i);
                    }
                }
            }
            var masterDetailXafGridView = Control as IColumnView;
            if (masterDetailXafGridView != null) masterDetailXafGridView.CanFilterGroupSummaryColumns = true;
        }
    }

    #region XAF GridLstEditor stuff
    public class GridListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        internal GridListEditorDynamicModelSynchronizer(GridListEditor columnViewEditor, IModelListView viewDesignMode,
                                                       bool overrideViewDesignMode)
            : this(columnViewEditor.GridView, (IModelListViewOptionsGridView)viewDesignMode, overrideViewDesignMode) {
            
        }

        public GridListEditorDynamicModelSynchronizer(XafGridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView) {
            var adapters =  modelListView.GridViewModelAdapters.SelectMany(adapter => adapter.ModelAdapters);
            foreach (var adapter in adapters){
                ModelSynchronizerList.Add(new GridListEditorViewOptionsSynchronizer(gridView, adapter, overrideViewDesignMode));    
            }
            ModelSynchronizerList.Add(new GridViewListEditorDynamicModelSynchronizer(gridView, modelListView,overrideViewDesignMode));
            ModelSynchronizerList.Add(new GridListEditorColumnOptionsSynchroniser(gridView, modelListView));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(gridView, modelListView));
        }

        public GridListEditorDynamicModelSynchronizer(GridListEditor columnViewEditor)
            : this(columnViewEditor, columnViewEditor.Model, false) {
        }

        public GridListEditorDynamicModelSynchronizer(object control, IModelNode model) : base(control, model) {
        }
    }

    public class GridListEditorViewOptionsSynchronizer :
        ComponentSynchronizer<XafGridView, IModelOptionsGridView> {
        public GridListEditorViewOptionsSynchronizer(GridListEditor control, bool overrideViewDesignMode)
            : base(control.GridView, ((IModelListViewOptionsGridView)control.Model).GridViewOptions, overrideViewDesignMode) {
        }

        public GridListEditorViewOptionsSynchronizer(XafGridView control, IModelOptionsGridView modelNode, bool overrideViewDesignMode) : base(control, modelNode, overrideViewDesignMode) {
        }
    }
    public class GridListEditorColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<GridListEditor, IModelListViewOptionsGridView, IModelColumnOptionsGridView> {
        readonly XafGridView _gridView;

        public GridListEditorColumnOptionsSynchroniser(GridListEditor control)
            : base(control, (IModelListViewOptionsGridView)control.Model) {
        }

        public GridListEditorColumnOptionsSynchroniser(XafGridView gridView, IModelListViewOptionsGridView modelNode) : this(new GridListEditor(modelNode)) {
            _gridView= gridView;
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return _gridView ?? Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsGridView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnGridView;
        }

    }

    #endregion

}
