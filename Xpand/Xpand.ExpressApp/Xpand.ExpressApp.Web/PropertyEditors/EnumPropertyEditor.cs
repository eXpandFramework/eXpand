using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Web.ASPxEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public class CheckboxEnumPropertyEditor : WebPropertyEditor {
        readonly Dictionary<ASPxCheckBox, int> controlsHash = new Dictionary<ASPxCheckBox, int>();
        readonly EnumDescriptor enumDescriptor;

        public CheckboxEnumPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {
            enumDescriptor = new EnumDescriptor(MemberInfo.MemberType);
        }

        public new Panel Editor {
            get { return (Panel)base.Editor; }
        }

        protected override WebControl CreateEditModeControlCore() {
            var placeHolder = new Panel();
            controlsHash.Clear();
            foreach (object enumValue in enumDescriptor.Values) {
                if ((int)enumValue != 0) {
                    var checkBox = new ASPxCheckBox();
                    controlsHash.Add(checkBox, (int)enumValue);
                    checkBox.Text = enumDescriptor.GetCaption(enumValue);
                    checkBox.CheckedChanged += checkBox_CheckedChanged;
                    placeHolder.Controls.Add(checkBox);
                }
            }
            return placeHolder;
        }

        void checkBox_CheckedChanged(object sender, EventArgs e) {
            EditValueChangedHandler(sender, e);
        }

        protected override string GetPropertyDisplayValue() {
            return enumDescriptor.GetCaption(PropertyValue);
        }

        protected override void ReadEditModeValueCore() {
            object value = PropertyValue;
            if (value != null) {
                foreach (ASPxCheckBox checkBox in Editor.Controls) {
                    int currentValue = controlsHash[checkBox];
                    checkBox.Checked = ((int)value & currentValue) != 0;
                }
            }
        }

        protected override object GetControlValueCore() {
            return Editor.Controls.Cast<ASPxCheckBox>().Where(checkBox => checkBox.Checked).Aggregate<ASPxCheckBox, object>(0, (current, checkBox) => (int)current | controlsHash[checkBox]);
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (Editor != null) {
                foreach (ASPxCheckBox checkBox in Editor.Controls) {
                    checkBox.CheckedChanged -= checkBox_CheckedChanged;
                }
                if (!unwireEventsOnly) {
                    controlsHash.Clear();
                }
            }
            base.BreakLinksToControl(unwireEventsOnly);
        }
    }
}

