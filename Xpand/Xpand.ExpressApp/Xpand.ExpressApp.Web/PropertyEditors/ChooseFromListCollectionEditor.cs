using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(IList<>), false)]
    public class ChooseFromListCollectionEditor : SerializedListPropertyEditor<object>, IChooseFromListCollectionEditor {
        public ChooseFromListCollectionEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
        }

        protected override bool IsMemberSetterRequired() {
            return (!MemberInfo.IsReadOnly || !MemberInfo.IsList) && base.IsMemberSetterRequired();
        }

        protected override void WriteValueCore() {
            var controlValueCore = GetControlValueCore();
            var strings = (controlValueCore + "").Split(ListBoxTemplate.SeparatorChar);
            var typeWrappers = GetDataSource().ToList();
            ((IList)PropertyValue).Clear();
            foreach (var typeWrapper in typeWrappers.Where(wrapper => strings.Contains(wrapper.ToString()))) {
                ((IList)PropertyValue).Add(typeWrapper);
            }
        }

        protected override WebControl CreateEditModeControlCore() {
            var editModeControlCore = base.CreateEditModeControlCore();
            ListBoxTemplate.PostValue = false;
            var values = ((IEnumerable<object>)PropertyValue).Select(wrapper => wrapper.ToString());
            var value = string.Join(ListBoxTemplate.SeparatorChar.ToString(CultureInfo.InvariantCulture), values);
            ListBoxTemplate.SetValue(value);
            foreach (var value1 in values) {
                ListBoxTemplate.Items.FindByText(value1).Selected = true;
            }
            Control.Value = value;
            return editModeControlCore;
        }

        protected override void ReadEditModeValueCore() {
            if (ASPxEditor != null) {
                ASPxEditor.Value = Control.Value;
            }
        }

        protected override string GetDisplayText(object item) {
            return item.ToString();
        }

        protected override object GetValue(object item) {
            return item;
        }

    }
}
