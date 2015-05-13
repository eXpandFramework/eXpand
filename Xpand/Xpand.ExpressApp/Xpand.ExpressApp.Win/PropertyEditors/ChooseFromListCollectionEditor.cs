using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Win.PropertyEditors {
    /// <summary>
    /// ChooseFromListCollectionEditor
    /// Assign this editor to a generic collection and it will retreive all items of that generic type, and list them in the combo box.
    /// it will then look at what items are in the collection and set the checkstate.
    /// if the checkstate of an item changes, it will be added or removed from the collection.  
    /// </summary>
    [PropertyEditor(typeof(IList<>), false)]
    public class ChooseFromListCollectionEditor : WinPropertyEditor, IChooseFromListCollectionEditor {
        private CheckedComboBoxEdit _comboControl;



        public ChooseFromListCollectionEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) {

        }

        #region Read what objects we've already go in out destination collection, and set the check state
        /// <summary>
        /// The value of the property is being read
        /// Read items in the destination list, and set the check value
        /// </summary>
        protected override void ReadValueCore() {
            var destinationList = PropertyValue as IEnumerable;
            if (destinationList != null) {
                _comboControl.EditValueChanged -= ComboControlEditValueChanged;
                SetCheckedItems(destinationList);
                _comboControl.EditValueChanged += ComboControlEditValueChanged;
            }
        }

        #endregion

        #region WriteValueCore
        /// <summary>
        /// Ensure that any selected or chcked items are in the destination list and unchecked are not
        /// </summary>
        protected override void WriteValueCore() {
            var destinationList = PropertyValue as IList;
            if (destinationList == null) {
                throw new UserFriendlyException(
                    new Exception("ChooseFromListCollectionEditor.WriteValueCore: Cannot get the destination list as an XPCollection."));
            }

            foreach (CheckedListBoxItemWrapper item in _comboControl.Properties.Items) {
                switch (item.CheckState) {
                    case CheckState.Checked:
                        if (!destinationList.Contains(item.O)) {
                            destinationList.Add(item.O);
                        }
                        break;
                    case CheckState.Unchecked:
                        if (destinationList.Contains(item.O)) {
                            destinationList.Remove(item.O);
                        }
                        break;
                }
            }
            View.ObjectSpace.SetModified(CurrentObject);
        }
        #endregion

        #region Create control
        /// <summary>
        /// Create control
        /// </summary>
        /// <returns></returns>
        protected override object CreateControlCore() {
            _comboControl = new CheckedComboBoxEdit();
            _comboControl.Properties.IncrementalSearch = true;
            return _comboControl;
        }

        #endregion

        #region OnControl Created
        /// <summary>
        /// Setup the control.
        /// </summary>
        protected override void OnControlCreated() {
            base.OnControlCreated();
            _comboControl.CustomDisplayText += ComboControlCustomDisplayText;
            PopulateCheckComboBox();
        }
        #endregion

        protected override object GetControlValueCore(){
            return _comboControl.EditValue;
        }

        void IPropertyEditor.SetValue(string value){
            foreach (var val in value.Split(Convert.ToChar(";"))){
                _comboControl.Properties.Items.Cast<CheckedListBoxItem>().First(item => (string) item.Value == val).CheckState =
                    CheckState.Checked;
            }
        }

        #region EditValue changed
        /// <summary>
        /// We've changed the checked value of an item
        /// Save the changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboControlEditValueChanged(object sender, EventArgs e) {
            WriteValueCore();
        }

        #endregion

        #region Custom Display text
        /// <summary>
        /// Display list of selected items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ComboControlCustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
            string captionText = string.Empty;
            foreach (CheckedListBoxItemWrapper item in _comboControl.Properties.Items) {
                if (item.CheckState == CheckState.Checked) {
                    if (captionText.Length > 0) captionText += ", ";
                    captionText += string.Format("{0}", item);
                }
            }
            e.DisplayText = captionText;
        }
        #endregion

        #region set Checked Item
        /// <summary>
        /// Mark already selected items in our check box.
        /// </summary>
        /// <param name="destinationList"></param>
        private void SetCheckedItems(IEnumerable destinationList) {
            ClearCheckMarks();
            _comboControl.Properties.Items.BeginUpdate();
            foreach (var o in destinationList) {
                CheckedListBoxItemWrapper found = FindComboItem(o);
                if (found != null) {
                    found.CheckState = CheckState.Checked;
                }
            }
            _comboControl.Properties.Items.EndUpdate();
        }

        #endregion

        #region ClearCheckMarks
        /// <summary>
        /// Clear checked items in drop down.
        /// </summary>
        private void ClearCheckMarks() {
            _comboControl.Properties.Items.BeginUpdate();
            foreach (CheckedListBoxItemWrapper item in _comboControl.Properties.Items) {
                item.CheckState = CheckState.Unchecked;
            }
            _comboControl.Properties.Items.EndUpdate();
        }

        #endregion

        #region FindComboItem
        /// <summary>
        /// Search for a combo item containing an IXPSimpleObject
        /// </summary>
        /// <param name="containingThisObject"></param>
        /// <returns></returns>
        private CheckedListBoxItemWrapper FindComboItem(object containingThisObject) {
            return _comboControl.Properties.Items.OfType<CheckedListBoxItemWrapper>().FirstOrDefault(item => Equals(item.O, containingThisObject));
        }

        #endregion

        #region Populate the combo box with items
        /// <summary>
        /// load combo box with available items to select.
        /// </summary>
        private void PopulateCheckComboBox() {
            _comboControl.Properties.Items.BeginUpdate();
            CheckedItems().ForEach(item => _comboControl.Properties.Items.Add(item));
            _comboControl.Properties.Items.EndUpdate();
        }

        List<CheckedListBoxItemWrapper> CheckedItems() {
            return GetAvaliableItems().OfType<object>().Select(o => new CheckedListBoxItemWrapper(FormatedValue(o), o, false)).ToList();
        }

        string FormatedValue(object o) {
            return string.IsNullOrEmpty(Model.DisplayFormat) ? o.ToString() : string.Format(Model.DisplayFormat, o);
        }

        class CheckedListBoxItemWrapper : CheckedListBoxItem {
            readonly object _o;

            public CheckedListBoxItemWrapper(string formatedValue, object o, bool isChecked)
                : base(formatedValue, isChecked) {
                _o = o;
            }

            public object O {
                get { return _o; }
            }
        }
        IEnumerable GetAvaliableItems() {
            var dataSourcePropertyAttribute = MemberInfo.FindAttribute<DataSourcePropertyAttribute>();
            if (dataSourcePropertyAttribute != null) {
                return (IEnumerable)MemberInfo.Owner.FindMember(dataSourcePropertyAttribute.DataSourceProperty).GetValue(CurrentObject);
            }
            return new List<object>();
        }
        #endregion
    }
}