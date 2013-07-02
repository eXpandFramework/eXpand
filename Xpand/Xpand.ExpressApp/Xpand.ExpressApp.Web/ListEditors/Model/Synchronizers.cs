using System.Linq;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web.ASPxGridView;
using Xpand.ExpressApp.Model.Options;
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
            foreach (var column in Model.Columns.OfType<IModelColumnOptionsGridView>()) {
                if (column.OptionsColumnGridView.NodeEnabled)
                    ApplyModel(column.OptionsColumnGridView, Control.Columns[column.PropertyName], ApplyValues);
            }
        }

        public override void SynchronizeModel() {

        }
    }
}
