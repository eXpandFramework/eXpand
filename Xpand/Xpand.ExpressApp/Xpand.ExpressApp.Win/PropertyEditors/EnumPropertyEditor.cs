﻿using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [PropertyEditor(typeof(Enum), true)]
    public class EnumPropertyEditor : DXPropertyEditor {
        EnumDescriptor _enumDescriptor;
        object _noneValue;

        public EnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            ImmediatePostData = model.ImmediatePostData;
        }

        bool TypeHasFlagsAttribute() {
            return GetUnderlyingType().GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
        }

        public new PopupBaseEdit Control {
            get { return (PopupBaseEdit)base.Control; }
        }
        protected override object CreateControlCore() {
            return TypeHasFlagsAttribute() ? (object)new CheckedComboBoxEdit() : new EnumEdit(MemberInfo.MemberType);
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return TypeHasFlagsAttribute()
                       ? (RepositoryItem)new RepositoryItemCheckedComboBoxEdit()
                       : new RepositoryItemEnumEdit(MemberInfo.MemberType);
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            if (TypeHasFlagsAttribute()) {
                _enumDescriptor = new EnumDescriptor(GetUnderlyingType());
                var checkedItem = ((RepositoryItemCheckedComboBoxEdit)item);
                checkedItem.BeginUpdate();
                checkedItem.Items.Clear();
                _noneValue = GetNoneValue();
                //checkedItem.SelectAllItemVisible = false;
                //Dennis: this is required to show localized items in the editor.
                foreach (object value in _enumDescriptor.Values)
                    if (!IsNoneValue(value))
                        checkedItem.Items.Add(value, _enumDescriptor.GetCaption(value), CheckState.Unchecked, true);
                //Dennis: use this method if you don't to show localized items in the editor.
                //checkedItem.SetFlags(GetUnderlyingType());
                checkedItem.EndUpdate();
                checkedItem.ParseEditValue += checkedEdit_ParseEditValue;
                checkedItem.CustomDisplayText += checkedItem_CustomDisplayText;
            }
        }

        void checkedEdit_ParseEditValue(object sender, ConvertEditValueEventArgs e) {
            if (string.IsNullOrEmpty(Convert.ToString(e.Value))) {
                ((CheckedComboBoxEdit)sender).EditValue = _noneValue;
                e.Handled = true;
            }
        }

        void checkedItem_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
            if (!IsNoneValue(e.Value) || _enumDescriptor == null) return;
            e.DisplayText = _enumDescriptor.GetCaption(e.Value);
        }

        bool IsNoneValue(object value) {
            if (value is string) return false;
            int result = int.MinValue;
            try {
                result = Convert.ToInt32(value);
            } catch {
            }
            return 0.Equals(result);
        }

        object GetNoneValue() {
            return Enum.ToObject(GetUnderlyingType(), 0);
        }
    }
}