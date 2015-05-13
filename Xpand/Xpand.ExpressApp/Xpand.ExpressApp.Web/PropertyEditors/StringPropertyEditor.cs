using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Fasterflect;
using Xpand.ExpressApp.PropertyEditors;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(string),true)]
    public class ASPxStringPropertyEditor : DevExpress.ExpressApp.Web.Editors.ASPx.ASPxStringPropertyEditor, IStringPropertyEditor,IPropertyEditor {
        public ASPxStringPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override WebControl CreateEditModeControlCore(){
            this.SetFieldValue("predefinedValues", Model.PredefinedValues);
            return base.CreateEditModeControlCore();
        }

        void IPropertyEditor.SetValue(string value){
            if (Editor != null) Editor.Text = value;
        }
    }
}
