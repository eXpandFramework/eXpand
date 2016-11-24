using System.Linq;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;
using GridViewModelSynchronizer = Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model.GridViewModelSynchronizer;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model {
    public class GridViewListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        public GridViewListEditorDynamicModelSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView) {
            ModelSynchronizerList.Add(new GridViewViewOptionsSynchronizer(gridView, modelListView, overrideViewDesignMode));
            ModelSynchronizerList.Add(new GridViewColumnOptionsSynchroniser(gridView, modelListView));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(gridView, modelListView));
        }
    }

    public class GridViewViewOptionsSynchronizer : ComponentSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelOptionsColumnView> {
        public GridViewViewOptionsSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsColumnView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView.BandsLayout.Enable? (IModelOptionsColumnView) ((IModelListViewOptionsAdvBandedView) modelListView).OptionsAdvBandedView:((IModelListViewOptionsGridView) modelListView).GridViewOptions, overrideViewDesignMode) {
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
            return modelColumnOptionsView.GetParent<IModelListView>().BandsLayout.Enable?
                (IModelColumnViewColumnOptions) ((IModelColumnOptionsAdvBandedView) modelColumnOptionsView).OptionsColumnAdvBandedView:
            modelColumnOptionsView.OptionsColumnGridView;
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

    #region XAF GridLstEditor stuff
    public class GridListEditorDynamicModelSynchronizer : ModelListSynchronizer {
        internal GridListEditorDynamicModelSynchronizer(GridListEditor columnViewEditor, IModelListView viewDesignMode,
                                                       bool overrideViewDesignMode)
            : this(columnViewEditor.GridView, (IModelListViewOptionsGridView)viewDesignMode, overrideViewDesignMode) {
            
        }

        public GridListEditorDynamicModelSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelListView, bool overrideViewDesignMode)
            : base(gridView, modelListView) {
            if (!modelListView.BandsLayout.Enable){
                var adapters =  modelListView.GridViewModelAdapters.SelectMany(adapter => adapter.ModelAdapters);
                foreach (var adapter in adapters){
                    if (modelListView.GridViewModelAdapters.Any(modelAdapter => modelAdapter.ModelAdapter==adapter))
                        ModelSynchronizerList.Add(new GridListEditorViewOptionsSynchronizer(gridView, adapter, overrideViewDesignMode));    
                }
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
        ComponentSynchronizer<DevExpress.XtraGrid.Views.Grid.GridView, IModelOptionsGridView> {
        public GridListEditorViewOptionsSynchronizer(GridListEditor control, bool overrideViewDesignMode)
            : base(control.GridView, ((IModelListViewOptionsGridView)control.Model).GridViewOptions, overrideViewDesignMode) {
        }

        public GridListEditorViewOptionsSynchronizer(DevExpress.XtraGrid.Views.Grid.GridView control, IModelOptionsGridView modelNode, bool overrideViewDesignMode) : base(control, modelNode, overrideViewDesignMode) {
        }
    }
    public class GridListEditorColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<GridListEditor, IModelListViewOptionsGridView, IModelColumnOptionsGridView> {
        readonly DevExpress.XtraGrid.Views.Grid.GridView _gridView;

        public GridListEditorColumnOptionsSynchroniser(GridListEditor control)
            : base(control, (IModelListViewOptionsGridView)control.Model) {
        }

        public GridListEditorColumnOptionsSynchroniser(DevExpress.XtraGrid.Views.Grid.GridView gridView, IModelListViewOptionsGridView modelNode) : this(new GridListEditor(modelNode)) {
            _gridView= gridView;
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return _gridView ?? Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsGridView modelColumnOptionsView) {
            return modelColumnOptionsView.GetParent<IModelListView>().BandsLayout.Enable ?(IModelColumnViewColumnOptions)((IModelColumnOptionsAdvBandedView)modelColumnOptionsView).OptionsColumnAdvBandedView :modelColumnOptionsView.OptionsColumnGridView;
        }

    }

    #endregion

}
