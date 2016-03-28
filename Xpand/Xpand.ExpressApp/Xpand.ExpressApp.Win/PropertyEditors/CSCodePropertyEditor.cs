using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.ExpressApp.Win.PropertyEditors.RichEdit;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;


namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof (string), EditorAliases.CSCodePropertyEditor, false)]
    [RichEditPropertyEditor("cs", false, false,"ControlText")]
    public class CSCodePropertyEditor : RichEditWinPropertyEditor {
        public CSCodePropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override object CreateControlCore(){
            var controlCore = base.CreateControlCore();
            ApplyMinimalConfiiguration((RichEditContainer) controlCore);
            return controlCore;
        }
    }
    
}