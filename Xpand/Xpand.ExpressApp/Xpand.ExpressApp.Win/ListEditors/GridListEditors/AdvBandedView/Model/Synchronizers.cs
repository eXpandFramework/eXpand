using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraGrid.Views.BandedGrid;
using Xpand.ExpressApp.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Design;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.Model;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.ColumnView.RepositoryItems;
using Xpand.ExpressApp.Win.ListEditors.GridListEditors.GridView.Model;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;
using Xpand.Utils.Linq;

namespace Xpand.ExpressApp.Win.ListEditors.GridListEditors.AdvBandedView.Model {
    public class AdvBandedViewLstEditorDynamicModelSynchronizer : ListEditorModelSynchronizer {
        public AdvBandedViewLstEditorDynamicModelSynchronizer(AdvBandedListEditor columnViewEditor)
            : base(columnViewEditor) {
            ModelSynchronizerList.Add(new XpandGridListEditorSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new AdvBandedViewOptionsSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new AdvBandedColumnOptionsSynchroniser(columnViewEditor));
            ModelSynchronizerList.Add(new AdvBandedViewGridBandsSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new XpandGridSummaryModelSynchronizer(columnViewEditor));
            ModelSynchronizerList.Add(new RepositoryItemColumnViewSynchronizer(columnViewEditor.GridView, columnViewEditor.Model));
        }
    }

    public class AdvBandedViewGridBandsSynchronizer : ModelSynchronizer<AdvBandedListEditor, IModelOptionsAdvBandedView> {
        public AdvBandedViewGridBandsSynchronizer(AdvBandedListEditor control)
            : base(control, control.Model.OptionsAdvBandedView) {
        }

        protected override void ApplyModelCore() {
            if (Model.NodeEnabled) {
                var modelGridBands = Model.GridBands.OrderBy(band => band.Index);
                ApplyModelBands(Control.GridView.Bands, modelGridBands);
            }
        }

        public override void SynchronizeModel() {
            if (Model.NodeEnabled) {

                foreach (AdvBandedGridColumn column in Control.GridView.Columns.OfType<AdvBandedGridColumn>()) {
                    if (column.OwnerBand == null && column.Model.GridBand != null)
                        column.Model.GridBand = null;
                    else if (column.OwnerBand != null && column.Model.GridBand != ((GridBand)column.OwnerBand).ModelGridBand) {
                        column.Model.GridBand = ((GridBand)column.OwnerBand).ModelGridBand;
                    }
                }
                SynchronizeGridBands(Control.GridView.Bands);
            }
        }

        void SynchronizeGridBands(GridBandCollection gridBands) {
            foreach (var gridBand in gridBands.OfType<GridBand>()) {
                gridBand.ModelGridBand.Index = gridBand.VisibleIndex;
                ApplyModel(gridBand.ModelGridBand, gridBand, SynchronizeValues);
                SynchronizeGridBands(gridBand.Children);
            }
        }

        GridBand GetGridBand(IModelGridBand modelGridBand, GridBandCollection bandCollection) {
            var gridBand = FindGridBand(modelGridBand);
            if (gridBand == null) {
                gridBand = new GridBand(modelGridBand) { Name = GetBandName(modelGridBand) };
                bandCollection.Add(gridBand);
                gridBand.Visible = modelGridBand.Index > -1;
            }
            ApplyModel(modelGridBand, gridBand, ApplyValues);
            return gridBand;
        }

        string GetBandName(IModelGridBand modelGridBand) {
            return modelGridBand.GetValue<string>("Id");
        }

        void ApplyModelBands(GridBandCollection bandCollection, IEnumerable<IModelGridBand> modelGridBands) {
            foreach (var modelGridBand in modelGridBands) {
                var gridBand = GetGridBand(modelGridBand, bandCollection);
                AddColumnToGridBand(gridBand, modelGridBand);
                ApplyModelBands(gridBand.Children, modelGridBand.GridBands);
            }
        }

        GridBand FindGridBand(IModelGridBand modelGridBand) {
            var gridBands = Control.GridView.Bands.GetItems<GridBand>(band => band.Children);
            return gridBands.FirstOrDefault(band => band.ModelGridBand == modelGridBand);
        }

        void AddColumnToGridBand(GridBand gridBand, IModelGridBand modelGridBand) {
            var bandedModelColumns = Control.Model.Columns.GetVisibleColumns().OfType<IModelColumnOptionsAdvBandedView>().Where(column => column.GridBand != null && GetBandName(column.GridBand) == GetBandName(modelGridBand));
            var columns = bandedModelColumns.OrderBy(view => view.OptionsColumnAdvBandedView.GetValue<int>("RowVIndex")).ThenBy(view => view.OptionsColumnAdvBandedView.GetValue<int>("ColVIndex"));
            foreach (var modelColumn in columns) {
                var bandedGridColumn = FindBandedGridColumn(modelColumn);
                if (bandedGridColumn != null) {
                    bandedGridColumn.OwnerBand = gridBand;
                    gridBand.Columns.Add(bandedGridColumn);
                }
            }
        }

        AdvBandedGridColumn FindBandedGridColumn(IModelColumnOptionsAdvBandedView modelColumn) {
            var bandedGridColumns = Control.GridView.Columns.OfType<AdvBandedGridColumn>();
            return bandedGridColumns.FirstOrDefault(column => column.PropertyName == modelColumn.PropertyName);
        }
    }

    public class AdvBandedViewOptionsSynchronizer : ComponentSynchronizer<DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView, IModelOptionsAdvBandedView> {
        public AdvBandedViewOptionsSynchronizer(AdvBandedListEditor control)
            : base(control.GridView, control.Model.OptionsAdvBandedView, ((IColumnViewEditor)control).OverrideViewDesignMode) {
        }
    }

    public class AdvBandedColumnOptionsSynchroniser : ColumnViewEditorColumnOptionsSynchronizer<AdvBandedListEditor, IModelListViewOptionsAdvBandedView, IModelColumnOptionsAdvBandedView> {
        public AdvBandedColumnOptionsSynchroniser(AdvBandedListEditor control)
            : base(control, control.Model) {
        }

        protected override DevExpress.XtraGrid.Views.Base.ColumnView GetColumnView() {
            return Control.GridView;
        }

        protected override IModelColumnViewColumnOptions GetColumnOptions(IModelColumnOptionsAdvBandedView modelColumnOptionsView) {
            return modelColumnOptionsView.OptionsColumnAdvBandedView;
        }

    }

}
