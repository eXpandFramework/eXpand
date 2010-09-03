using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Web.ListEditors {
    public class XpandASPxGridListEditorSynchronizer : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditorSynchronizer
    {
        public XpandASPxGridListEditorSynchronizer(DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor gridListEditor, IModelListView model) : base(gridListEditor, model) {
            Add(new GridViewOptionsModelSynchronizer(gridListEditor.Grid, model));
            foreach (var column in model.Columns) {
                Add(new ColumnOptionsModelSynchronizer(gridListEditor.Grid,column));
            }
        }
    }
}