using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Xpand.Persistent.Base.General.CustomAttributes;
using EditorAliases = Xpand.Persistent.Base.General.EditorAliases;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    [DevExpress.ExpressApp.Editors.PropertyEditor(typeof(Enum),EditorAliases.EnumPropertyEditor,true)]
    public class EnumPropertyEditor : DevExpress.ExpressApp.Win.Editors.EnumPropertyEditor,IComplexViewItem,IEnumPropertyEditor {
        EnumDescriptor _enumDescriptor;
        object _noneValue;
        private IObjectSpace _objectSpace;
        private ImageComboBoxItem[] _startitems;

        public EnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {            
            CurrentObjectChanged+=OnCurrentObjectChanged;
        }

        private void OnCurrentObjectChanged(object sender, EventArgs e) {
            if (Control != null) FilterRepositoryItem((RepositoryItemEnumEdit) Control.Properties);
        }

        bool TypeHasFlagsAttribute() {
            return GetUnderlyingType().GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0;
        }

        public new PopupBaseEdit Control => base.Control;

        protected override object CreateControlCore() {
            return TypeHasFlagsAttribute() ? new CheckedComboBoxEdit() : base.CreateControlCore();
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
                foreach (object value in _enumDescriptor.Values)
                    if (!IsNoneValue(value))
                        checkedItem.Items.Add(value, _enumDescriptor.GetCaption(value), CheckState.Unchecked, true);
                checkedItem.EndUpdate();
                checkedItem.ParseEditValue += checkedEdit_ParseEditValue;
                checkedItem.CustomDisplayText += checkedItem_CustomDisplayText;
            }

            _startitems = ((RepositoryItemComboBox) item).Items.Cast<ImageComboBoxItem>().ToArray();
            if (item is RepositoryItemEnumEdit repositoryItemEnumEdit) {
                FilterRepositoryItem(repositoryItemEnumEdit);
            }
        }

        private void FilterRepositoryItem(RepositoryItemEnumEdit repositoryItemEnumEdit){
            this.SetupDataSource(_startitems,repositoryItemEnumEdit.Items, item => item.Value);
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

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            base.BreakLinksToControl(unwireEventsOnly);
            CurrentObjectChanged-=OnCurrentObjectChanged;
            if (_objectSpace != null) _objectSpace.Committed -= ObjectSpaceOnCommitted;
        }

        bool IsNoneValue(object value) {
            if (value is string) return false;
            int result = int.MinValue;
            try {
                result = Convert.ToInt32(value);
            }
            catch {
                // ignored
            }

            return 0.Equals(result);
        }

        object GetNoneValue() {
            return Enum.ToObject(GetUnderlyingType(), 0);
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            _objectSpace = objectSpace;
            _objectSpace.Committed+=ObjectSpaceOnCommitted;
        }

        private void ObjectSpaceOnCommitted(object sender, EventArgs e) {
            if (Control != null) FilterRepositoryItem((RepositoryItemEnumEdit) Control.Properties);
        }
    }
}