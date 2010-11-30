using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace Xpand.ExpressApp.Web.ListEditors
{
    [ListEditor(typeof(object))]
    public class XpandASPxGridListEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor
    {
        public XpandASPxGridListEditor(IModelListView info) : base(info) {
        }
        protected override IModelSynchronizable CreateModelSynchronizer()
        {
            return new XpandASPxGridListEditorSynchronizer(this, Model);
        }
    }
}
