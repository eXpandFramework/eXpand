using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;


namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof (string), EditorAliases.CSCodePropertyEditor, false)]
    public class CSCodePropertyEditor : RichEditWinPropertyEditor {
        public CSCodePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        public override string GetRichEditHighLightExtension(){
            return "cs";
        }
    }
    
}