using System.Linq;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
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
            foreach (var viewDataColumnWithInfo in dataColumnWithInfos.Where(column => column.FieldName != "ProtectedContentColumn ED6F4AF3-F04C-45EB-B8C1-6CEE05D395B2")) {
                var modelColumnOptionsGridView = ((IModelColumnOptionsGridView) viewDataColumnWithInfo.Model(Model));
                ApplyModel(modelColumnOptionsGridView.OptionsColumnGridView, viewDataColumnWithInfo, ApplyValues);
            }
        }

        public override void SynchronizeModel() {

        }
    }
}
