using System;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(DateTime),EditorAliases.TimeSpanPropertyEditor,false)]
    public class ASPxTimePropertyEditor : ASPxPropertyEditor {
        private const string TimeFormat = "HH:mm";

        public ASPxTimePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }

        public new ASPxTimeEdit Editor {
            get { return (ASPxTimeEdit)base.Editor; }
        }
        private void SelectedDateChangedHandler(object source, EventArgs e) {
            FixYear(source as ASPxTimeEdit);

            EditValueChangedHandler(source, e);
        }

        protected override IJScriptTestControl GetEditorTestControlImpl() {
            return new JSASPxDateTestControl();
        }

        protected override string GetPropertyDisplayValue() {
            if (Equals(PropertyValue, DateTime.MinValue) || !(PropertyValue is DateTime)) {
                return string.Empty;
            }
            return ((DateTime)PropertyValue).ToString(TimeFormat);
        }
        protected override void SetupControl(WebControl control) {
            base.SetupControl(control);
            var aSPxDateEdit = control as ASPxTimeEdit;
            if (aSPxDateEdit != null) {
                aSPxDateEdit.EditFormat = EditFormat.Custom;
                aSPxDateEdit.EditFormatString = EditMask;
                aSPxDateEdit.DisplayFormatString = TimeFormat;
                aSPxDateEdit.DateChanged += SelectedDateChangedHandler;
            }
        }
        protected override void SetImmediatePostDataScript(string script) {
            Editor.ClientSideEvents.ValueChanged = script;
        }
        protected override WebControl CreateEditModeControlCore() {
            return new ASPxTimeEdit();
        }
        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (Editor != null) {
                Editor.DateChanged -= SelectedDateChangedHandler;
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }

        private void FixYear(ASPxTimeEdit editor) {
            if (editor == null)
                return;

            if (editor.Value is DateTime) {
                var time = (DateTime)editor.Value;
                editor.Value = time.AddYears(Math.Max(2000 - time.Year, 0));
            }
        }
    }
}
