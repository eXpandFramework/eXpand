using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Web;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(object), EditorAliases.LabelPropertyEditor, false)]
    public class ASPxLabelPropertyEditor:DevExpress.ExpressApp.Web.Editors.ASPx.ASPxStringPropertyEditor{
        public ASPxLabelPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
        }

        protected override WebControl CreateEditModeControlCore(){
            var textEdit = (ASPxTextEdit)base.CreateEditModeControlCore();
            textEdit.ReadOnlyStyle.Border.BorderWidth = 0;
            textEdit.ReadOnly = true;    
            return textEdit;
        }
    }
}
