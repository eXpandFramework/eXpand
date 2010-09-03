using DevExpress.ExpressApp.Model;

namespace eXpand.ExpressApp.Web.ListEditors
{
    public class XpandASPxGridListEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor
    {
        public XpandASPxGridListEditor(IModelListView info) : base(info) {
        }
        protected override DevExpress.ExpressApp.Core.ModelSynchronizer CreateModelSynchronizer()
        {
            return new XpandASPxGridListEditorSynchronizer(this, Model);
        }
    }
}
