using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Persistent.Base;
using DevExpress.Web.ASPxEditors;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    [PropertyEditor(typeof(Enum), EditorAliases.EnumPropertyEditor, false)]
    public class WebFilterableEnumPropertyEditor : ASPxEnumPropertyEditor, IComplexViewItem {
        static PropertyDescriptorCollection _propertyDescriptorCollection;
        readonly PropertyInfo _dataSourceProperty;
        readonly string _isNullCriteria;
        readonly DataSourcePropertyIsNullMode _isNullMode = DataSourcePropertyIsNullMode.SelectAll;
        readonly Type _propertyType;

        IObjectSpace _objectSpace;

        public WebFilterableEnumPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            PropertyInfo propertyInfo = ObjectType.GetProperty(PropertyName);
            if (propertyInfo != null) {
                _propertyType = propertyInfo.PropertyType;
                foreach (object item in propertyInfo.GetCustomAttributes(false)) {
                    var propAttr = item as DataSourcePropertyAttribute;
                    if (propAttr != null && !string.IsNullOrEmpty(propAttr.DataSourceProperty)) {
                        PropertyInfo dataSourceProperty = ObjectType.GetProperty(propAttr.DataSourceProperty);
                        _isNullMode = propAttr.DataSourcePropertyIsNullMode;
                        _isNullCriteria = propAttr.DataSourcePropertyIsNullCriteria;
                        if (dataSourceProperty != null) {
                            if (typeof(IEnumerable).IsAssignableFrom(dataSourceProperty.PropertyType) &&
                                dataSourceProperty.PropertyType.IsGenericType &&
                                dataSourceProperty.PropertyType.GetGenericArguments()[0].IsAssignableFrom(
                                    propertyInfo.PropertyType))
                                _dataSourceProperty = dataSourceProperty;
                        }
                    }

                    var criteriaAttr = item as DataSourceCriteriaAttribute;
                    if (criteriaAttr != null)
                        _isNullCriteria = criteriaAttr.DataSourceCriteria;
                }
            }
        }

        protected static PropertyDescriptorCollection PropertyDescriptorCollection {
            get {
                return _propertyDescriptorCollection ??
                       (_propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(ListEditItem)));
            }
        }
        #region IComplexPropertyEditor Members
        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            if (_objectSpace != null)
                _objectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            _objectSpace = objectSpace;
            _objectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
        }
        #endregion
        protected override void SetupControl(WebControl control) {
            var editor = control as ASPxComboBox;
            if (editor != null) {
                editor.ShowImageInEditBox = true;
                editor.SelectedIndexChanged += ExtendedEditValueChangedHandler;
                FillEditor(editor);
            }
        }

        void FillEditor(ASPxComboBox editor) {
            if (editor == null) return;
            editor.Items.Clear();
            editor.ValueType = GetComboBoxValueType();

            IEnumerable dataSource = GetDataSource();

            if (_propertyType.IsGenericType && _propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                editor.Items.Add(CaptionHelper.NullValueText, null);
            }

            if (dataSource != null)
                foreach (object value in dataSource) {
                    if (value is ListEditItem)
                        editor.Items.Add(value as ListEditItem);
                    else
                        editor.Items.Add(CreateEditItem(value));
                }
        }

        IEnumerable GetDataSource() {
            IEnumerable dataSource = null;
            if (_dataSourceProperty != null) {
                dataSource = _dataSourceProperty.GetValue(CurrentObject, null) as IEnumerable;
                if (dataSource != null) {
                    bool hasItems = (dataSource).GetEnumerator().MoveNext();
                    if (!hasItems)
                        dataSource = null;
                }
            }
            if (dataSource == null) {
                if (string.IsNullOrEmpty(_isNullCriteria)) {
                    if (_isNullMode == DataSourcePropertyIsNullMode.SelectAll)
                        return descriptor.Values;
                    if (_isNullMode == DataSourcePropertyIsNullMode.SelectNothing)
                        return null;
                } else {
                    CriteriaOperator criteriaOperator = CriteriaOperator.Parse(_isNullCriteria);
                    if (!ReferenceEquals(criteriaOperator, null)) {
                        dataSource = new List<object>();
                        foreach (object item in descriptor.Values)
                            ((IList)dataSource).Add(CreateEditItem(item));
                        criteriaOperator.Accept(new EnumCriteriaParser(PropertyName, _propertyType));
                        var filteredDataSource = new ExpressionEvaluator(PropertyDescriptorCollection, criteriaOperator, true).Filter(dataSource);
                        ((IList)dataSource).Clear();
                        foreach (ListEditItem item in filteredDataSource)
                            ((IList)dataSource).Add(item);
                    }
                }
            }
            return dataSource;
        }

        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (e.PropertyName == _dataSourceProperty.Name) {
                FillEditor(Editor);
            }
        }

        ListEditItem CreateEditItem(object enumValue) {
            object value = ConvertEnumValueForComboBox(enumValue);
            ImageInfo imageInfo = GetImageInfo(enumValue);
            if (imageInfo.IsUrlEmpty) {
                return new ListEditItem(descriptor.GetCaption(enumValue), value);
            }
            return new ListEditItem(descriptor.GetCaption(enumValue), value, imageInfo.ImageUrl);
        }
    }

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

