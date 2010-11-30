using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.ListEditors {
    public class XpandASPxGridListEditorSynchronizer : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditorSynchronizer
    {
        private ModelSynchronizerList modelSynchronizerList;

        public XpandASPxGridListEditorSynchronizer(DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            modelSynchronizerList = new ModelSynchronizerList();
            modelSynchronizerList.Add(new GridViewOptionsModelSynchronizer(gridListEditor.Grid, model));
            foreach (var column in model.Columns) {
                modelSynchronizerList.Add(new ColumnOptionsModelSynchronizer(gridListEditor.Grid, column));
            }
        }

        protected override void ApplyModelCore() {
            base.ApplyModelCore();
            modelSynchronizerList.ApplyModel();
        }

        public override void SynchronizeModel() {
            base.SynchronizeModel();
            modelSynchronizerList.SynchronizeModel();
        }
    }
}