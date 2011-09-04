using System;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public class ASPxStringPropertyEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxStringPropertyEditor, IStringPropertyEditor {
        public ASPxStringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }
    }
}
