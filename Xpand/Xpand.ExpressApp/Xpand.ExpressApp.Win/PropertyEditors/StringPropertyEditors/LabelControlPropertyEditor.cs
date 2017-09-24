using System;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.Persistent.Base.General;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors.StringPropertyEditors {

    [PropertyEditor(typeof(object),EditorAliases.LabelPropertyEditor,false)]
    public class LabelControlPropertyEditor : WinPropertyEditor, IPropertyEditor{

        public LabelControlPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model){
            ControlBindingProperty = "Text";
        }

        public override bool CanFormatPropertyValue => true;

        protected override object CreateControlCore(){
            var labelControl = new LabelControl{
                BorderStyle = BorderStyles.NoBorder,
                AutoSizeMode = LabelAutoSizeMode.None,
                ShowLineShadow = false
            };

            return labelControl;
        }

        protected override void ReadValueCore(){
            base.ReadValueCore();
            Control.Text = string.Format(DisplayFormat,PropertyValue);
        }

        void IPropertyEditor.SetValue(string value){
            Control.Text = value;
        }
    }
}
