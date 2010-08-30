using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Web.ListEditors
{
    public class ASPxGridListEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor
    {
        public ASPxGridListEditor(IModelListView info) : base(info) {
        }
        protected override DevExpress.ExpressApp.Core.ModelSynchronizer CreateModelSynchronizer()
        {
            return new ASPxGridListEditorSynchronizer(this, Model);
        }
    }
}
