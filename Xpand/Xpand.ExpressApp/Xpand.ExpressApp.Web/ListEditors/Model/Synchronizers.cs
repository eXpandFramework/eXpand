using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridViewListEditorModelSynchronizer:ModelListSynchronizer {
        public GridViewListEditorModelSynchronizer(ASPxGridListEditor columnViewEditor)
            : base(columnViewEditor, columnViewEditor.Model){
            var listViewOptionsGridView = (IModelListViewOptionsGridView)columnViewEditor.Model;
            foreach (var modelAdapter in listViewOptionsGridView.GridViewModelAdapters.SelectMany(adapter => adapter.ModelAdapters)){
                ModelSynchronizerList.Add(new GridViewListViewOptionsSynchronizer(columnViewEditor.Grid, modelAdapter));
            }
            ModelSynchronizerList.Add(new GridViewListViewOptionsSynchronizer(columnViewEditor.Grid, ((IModelListViewOptionsGridView)columnViewEditor.Model).GridViewOptions));
            ModelSynchronizerList.Add(new GridViewListColumnOptionsSynchronizer(columnViewEditor.Grid, listViewOptionsGridView));
        }
    }

    public class GridViewListViewOptionsSynchronizer : ModelSynchronizer<ASPxGridView, IModelOptionsGridView> {
        public GridViewListViewOptionsSynchronizer(ASPxGridView component, IModelOptionsGridView modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            if (Model.NodeEnabled)
                ApplyModel(Model, Control, ApplyValues);
        }

        public override void SynchronizeModel() {

        }
    }

    public class GridViewListColumnOptionsSynchronizer : ModelSynchronizer<ASPxGridView, IModelListViewOptionsGridView> {
        public GridViewListColumnOptionsSynchronizer(ASPxGridView component, IModelListViewOptionsGridView modelNode)
            : base(component, modelNode) {
        }

        protected override void ApplyModelCore() {
            var dataColumnWithInfos = Control.Columns.OfType<GridViewDataColumn>().ToList();
            foreach (var viewDataColumnWithInfo in dataColumnWithInfos) {
                var modelColumnOptionsGridView = ((IModelColumnOptionsGridView) viewDataColumnWithInfo.Model(Model));
                ApplyModel(modelColumnOptionsGridView.OptionsColumnGridView, viewDataColumnWithInfo, ApplyValues);
            }
            ApplyGridBandModel(dataColumnWithInfos);

        }

        void ApplyGridBandModel(List<GridViewDataColumn> dataColumnWithInfos) {
            var modelColumnOptionsGridViewBands =
                Model.Columns.OfType<IModelColumnOptionsGridViewBand>().Where(band => band.GridViewBand != null);
            if (modelColumnOptionsGridViewBands.Any()) {
                Control.Columns.Clear();
            }
            foreach (var column in modelColumnOptionsGridViewBands) {
                if (column.OptionsColumnGridView.NodeEnabled) {
                    var gridViewColumn = dataColumnWithInfos.Single(info => info.Model(Model) == column);
                    ApplyModel(column.OptionsColumnGridView, gridViewColumn, ApplyValues);
                    var modelGridViewBand = column.GridViewBand;
                    if (modelGridViewBand != null) {
                        var name = modelGridViewBand.GetValue<string>("Name");
                        GridViewBandColumn gridViewBandColumn;
                        if (Control.Columns[name] == null) {
                            gridViewBandColumn = new GridViewBandColumn{Name = name};
                            ApplyModel(modelGridViewBand, gridViewBandColumn, ApplyValues);
                            Control.Columns.Add(gridViewBandColumn);
                        }
                        else gridViewBandColumn = (GridViewBandColumn) Control.Columns[name];

                        gridViewBandColumn.Columns.Add(gridViewColumn);
//                        ContainerCell.Columns.Remove(gridViewColumn);
                    }
                }
            }
            if (modelColumnOptionsGridViewBands.Any()) {
                Control.DataBind();
            }
        }

        public override void SynchronizeModel() {

        }
    }
}
