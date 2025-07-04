using System;
using System.Collections;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using EnumsNET;
using Xpand.Persistent.Base.General;
using Xpand.Persistent.Base.General.CustomAttributes;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(Enum),EditorAliases.EnumPropertyEditor,false)]
    public class EnumPropertyEditor : DevExpress.ExpressApp.Win.Editors.EnumPropertyEditor,IComplexViewItem,IEnumPropertyEditor {
        EnumDescriptor _enumDescriptor;
        object _noneValue;
        private IObjectSpace _objectSpace;
        private object _control;
        private (ImageComboBoxItem[] startComboBoxItems, CheckedListBoxItem[] startCheckedListBoxItems) _itemsData;
        private bool _objectChanged;

        public EnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {            
            CurrentObjectChanged+=OnCurrentObjectChanged;
        }

        private void OnCurrentObjectChanged(object sender, EventArgs e) {
            if (Control != null) FilterRepositoryItem( Control.Properties,MemberInfo,CurrentObject,_objectSpace, _itemsData);
        }

        public static bool TypeHasFlagsAttribute(IMemberInfo info) {
            return PropertyEditorHelper.CalcUnderlyingType(info).GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
        }

        public new PopupBaseEdit Control => (PopupBaseEdit) _control;

        protected override object CreateControlCore() {
            _control = (Control) (TypeHasFlagsAttribute(MemberInfo) ? new CheckedComboBoxEdit() : base.CreateControlCore());
            return _control;
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return TypeHasFlagsAttribute(MemberInfo)
                       ? (RepositoryItem)new RepositoryItemCheckedComboBoxEdit()
                       : new RepositoryItemEnumEdit(MemberInfo.MemberType);
        }

        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            _objectSpace.ObjectChanged+=ObjectSpaceOnObjectChanged;

            if (TypeHasFlagsAttribute(MemberInfo)) {
                _enumDescriptor = new EnumDescriptor(GetUnderlyingType());
                var comboBoxEdit = ((RepositoryItemCheckedComboBoxEdit)item);
                comboBoxEdit.BeginUpdate();
                comboBoxEdit.Items.Clear();
                _noneValue = GetNoneValue();
                foreach (object value in _enumDescriptor.Values)
                    comboBoxEdit.Items.Add(value, _enumDescriptor.GetCaption(value), CheckState.Unchecked, true);
                SetCheckState(MemberInfo, PropertyValue, comboBoxEdit.Items.ToArray());
                comboBoxEdit.EndUpdate();
                comboBoxEdit.ParseEditValue += checkedEdit_ParseEditValue;
                comboBoxEdit.CustomDisplayText += checkedItem_CustomDisplayText;
            }

            _itemsData = GetItemsData(item,MemberInfo);
            FilterRepositoryItem(item,MemberInfo, CurrentObject,_objectSpace,_itemsData);
        }

        private  static void SetCheckState(IMemberInfo memberInfo,object value,CheckedListBoxItem[] items) {
            var flags = FlagEnums.GetFlags(memberInfo.MemberType, value).ToArray();
            foreach (CheckedListBoxItem item in items) {
                item.CheckState = flags.Contains(item.Value) ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        public static (ImageComboBoxItem[] startComboBoxItems, CheckedListBoxItem[] startCheckedListBoxItems) GetItemsData(RepositoryItem repositoryItem,IMemberInfo memberInfo) {
            if (TypeHasFlagsAttribute(memberInfo)) {
                return (null, ((RepositoryItemCheckedComboBoxEdit) repositoryItem).Items.ToArray());
            }

            return (((RepositoryItemComboBox) repositoryItem).Items.Cast<ImageComboBoxItem>().ToArray(),null);
        }


        private void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs e) {
            if ((e.MemberInfo != null && e.MemberInfo != MemberInfo||e.PropertyName!=null&&e.PropertyName!=PropertyName)&& Control != null) {
                _objectChanged = true;
                FilterRepositoryItem(Control.Properties,MemberInfo,CurrentObject,_objectSpace, _itemsData);
                _objectChanged = false;
            }
        }

        public static void FilterRepositoryItem( RepositoryItem repositoryItem, IMemberInfo memberInfo,object objectInstance,IObjectSpace objectSpace,
            (ImageComboBoxItem[] startComboBoxItems, CheckedListBoxItem[] startCheckedListBoxItems) items) {
            IList controlItems;
            if (repositoryItem is RepositoryItemEnumEdit edit) {
                controlItems = edit.Items;
                memberInfo.SetupEnumPropertyDataSource(objectInstance,objectSpace,  items.startComboBoxItems,controlItems, item => item.Value);
            }
            else {
                controlItems = ((RepositoryItemCheckedComboBoxEdit) repositoryItem).Items;
                var isModified = objectSpace.IsModified;
                memberInfo.SetupEnumPropertyDataSource(objectInstance,objectSpace, items.startCheckedListBoxItems,controlItems, item => item.Value);
                objectSpace.SetIsModified(isModified);
                SetCheckState(memberInfo, memberInfo.GetValue(objectInstance),items.startCheckedListBoxItems);
            }
            
        }

        void checkedEdit_ParseEditValue(object sender, ConvertEditValueEventArgs e) {
            if (_objectChanged) {
                return;
            }
            if (string.IsNullOrEmpty(Convert.ToString(e.Value))) {
                ((CheckedComboBoxEdit)sender).EditValue = _noneValue;
                e.Handled = true;
            }

            var flagsValues = $"{((CheckedComboBoxEdit)sender).EditValue}".Split(',').Select(_ => _.Trim())
                .Select(_ => EnumsNET.Enums.Parse(MemberInfo.MemberType,_));
            var flags = FlagEnums.CombineFlags(MemberInfo.MemberType,flagsValues);
            PropertyValue = flags;
        }

        void checkedItem_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
//            if (!IsNoneValue(e.Value) || _enumDescriptor == null) return;
//            e.DisplayText = _enumDescriptor.GetCaption(e.Value);
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            base.BreakLinksToControl(unwireEventsOnly);
            CurrentObjectChanged-=OnCurrentObjectChanged;
            if (_objectSpace != null) {
                _objectSpace.Committed -= ObjectSpaceOnCommitted;
                _objectSpace.ObjectChanged-=ObjectSpaceOnObjectChanged;
            }
        }

//        bool IsNoneValue(object value) {
//            if (value is string) return false;
//            int result = int.MinValue;
//            try {
//                result = Convert.ToInt32(value);
//            }
//            catch {
//                // ignored
//            }
//
//            return 0.Equals(result);
//        }

        object GetNoneValue() {
            return Enum.ToObject(GetUnderlyingType(), 0);
        }

        void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _objectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs e) {
            if (Control != null) FilterRepositoryItem( Control.Properties,MemberInfo,CurrentObject,_objectSpace,  _itemsData);
        }
    }
}