using System.Linq;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using Xpand.Persistent.Base.General.Model.Options;
using Xpand.Persistent.Base.ModelAdapter;

namespace Xpand.ExpressApp.Web.ListEditors.Model {
    public class GridViewListEditorModelSynchronizer : ModelListSynchronizer {
        public GridViewListEditorModelSynchronizer(ASPxGridListEditor columnViewEditor): this(columnViewEditor.Grid, ((IModelListViewOptionsGridView)columnViewEditor.Model).GridViewOptions) {
            ModelSynchronizerList.Add(new GridViewListColumnOptionsSynchronizer(columnViewEditor.Grid, (IModelListViewOptionsGridView) columnViewEditor.Model));
        }

        public GridViewListEditorModelSynchronizer(ASPxGridView asPxGridView, IModelOptionsGridView modelOptionsGridView): base(asPxGridView, modelOptionsGridView) {
            ModelSynchronizerList.Add(new GridViewListViewOptionsSynchronizer(asPxGridView, modelOptionsGridView));
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
            var sources = Control.Columns.OfType<GridViewDataColumnWithInfo>().ToList();
            
            var modelColumnOptionsGridViewBands = Model.Columns.OfType<IModelColumnOptionsGridViewBand>().Where(band => band.GridViewBand != null);
            if (modelColumnOptionsGridViewBands.Any()) {
                Control.Columns.Clear();    
            }
            foreach (var column in modelColumnOptionsGridViewBands) {
                if (column.OptionsColumnGridView.NodeEnabled) {
                    var gridViewColumn = sources.Single(info => info.Model==column);
                    ApplyModel(column.OptionsColumnGridView, gridViewColumn, ApplyValues);
                    var modelGridViewBand = column.GridViewBand;
                    if (modelGridViewBand != null) {
                        var name = modelGridViewBand.GetValue<string>("Name");
                        GridViewBandColumn gridViewBandColumn;
                        if (Control.Columns[name] == null) {
                            gridViewBandColumn = new GridViewBandColumn{Name = name};
                            ApplyModel(modelGridViewBand, gridViewBandColumn,ApplyValues);
                            Control.Columns.Add(gridViewBandColumn);
                        }
                        else gridViewBandColumn = (GridViewBandColumn) Control.Columns[name];
                        
                        gridViewBandColumn.Columns.Add(gridViewColumn);
//                        Control.Columns.Remove(gridViewColumn);
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
