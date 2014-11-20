using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;

namespace Xpand.ExpressApp.Web.PropertyEditors {
    public class SerializedListBoxTemplate : ASPxListBox, ITemplate {
        bool _postValue=true;

        public SerializedListBoxTemplate() {
            SelectionMode = ListEditSelectionMode.CheckColumn;
            EnableClientSideAPI = true;
        }

        public bool PostValue {
            get { return _postValue; }
            set { _postValue = value; }
        }

        public override Unit Width {
            get { return Unit.Percentage(100.0); }
            set {  }
        }

        public override Unit Height {
            get { return 300; }
            set {  }
        }

        private string _dropDownId;
        private char _separatorChar = ',';

        public char SeparatorChar {
            get { return _separatorChar; }
        }

        public void InstantiateIn(Control container) {
            InitClientSideEvents();
            container.Controls.Add(this);
        }

        private void InitClientSideEvents() {
            ClientSideEvents.Init =
                @"function (s, args) {
                    var listBox = ASPxClientControl.Cast(s);
                    listBox.autoResizeWithContainer = true;
                }";

            ClientSideEvents.SelectedIndexChanged =
                @"function (s, args) {
                    var listBox = ASPxClientControl.Cast(s);
                    var checkComboBox = ASPxClientControl.Cast(" + _dropDownId + @");

                    var selectedItems = listBox.GetSelectedItems();
                    " + SetPostData()+ @"
                }";
        }

        string SetPostData() {
            return _postValue? GetPostValueJs(): GetPostTextJs();
        }

        string GetPostTextJs() {
            return @"var values = [];
                    for(var i = 0; i < selectedItems.length; i++)
                        values.push(selectedItems[i].text);

                    checkComboBox.SetText(values.join('" + _separatorChar + @"'));";
        }

        string GetPostValueJs() {
            return @"var values = [];
                    for(var i = 0; i < selectedItems.length; i++)
                        values.push(selectedItems[i].value);

                    checkComboBox.SetValue(values.join('" + _separatorChar + @"'));";
        }

        public void SetDropDownId(string id) {
            _dropDownId = id;
        }

        public void SetSeparatorChar(char separatorChar) {
            _separatorChar = separatorChar;
        }

        public void SetValue(string value) {
            foreach (ListEditItem item in Items) {
                item.Selected = value.Contains(item.Value.ToString());
            }
        }
    }

    public abstract class SerializedListPropertyEditor<T> : ASPxPropertyEditor, IComplexViewItem {
        public class ListBoxItem {
            public string DisplayText { get; set; }
            public object Value { get; set; }
        }

        protected SerializedListPropertyEditor(Type objectType, IModelMemberViewItem info)
            : base(objectType, info) { }

        protected virtual IEnumerable<T> GetDataSource() {
            if (Helper.IsPropertyDataSource) {
                var dataSource = Helper.CreateCollectionSource(CurrentObject);
                var enumerable = ((IEnumerable)dataSource.Collection);
                return enumerable.Cast<T>();
            }
            return ObjectSpace.GetObjects<T>(CriteriaOperator.Parse(Model.ModelMember.DataSourceCriteria));
        }

        private IEnumerable<ListBoxItem> GetListBoxItems() {
            IEnumerable<T> dataSource = GetDataSource();
            return dataSource
                .Select(_ =>
                    new ListBoxItem{
                        DisplayText = GetDisplayText(_),
                        Value = GetValue(_)
                    })
                .OrderBy(_ => _.DisplayText);
        }

        protected abstract string GetDisplayText(T item);
        protected abstract object GetValue(T item);

        public IObjectSpace ObjectSpace { get; private set; }
        public new ASPxDropDownEdit Control { get; private set; }
        public LookupEditorHelper Helper { get; private set; }

        private SerializedListBoxTemplate _listBoxTemplate;
        public SerializedListBoxTemplate ListBoxTemplate {
            get { return _listBoxTemplate ?? (_listBoxTemplate = new SerializedListBoxTemplate()); }
        }

        private void PopulateListBoxItems() {
            ListBoxTemplate.Items.Clear();
            foreach (var item in GetListBoxItems()) {
                ListBoxTemplate.Items.Add(item.DisplayText, item.Value);
            }
        }

        protected override WebControl CreateEditModeControlCore() {
            Control = new ASPxDropDownEdit();
            Control.ValueChanged += ExtendedEditValueChangedHandler;
            Control.EnableClientSideAPI = true;
            Control.DropDownWindowTemplate = ListBoxTemplate;
            Control.ClientInstanceName = "ListPropertyEditor_" + PropertyName;
            Control.ReadOnly = true;

            ListBoxTemplate.SetDropDownId(Control.ClientInstanceName);
            PopulateListBoxItems();

            if (PropertyValue != null)
                ListBoxTemplate.SetValue(PropertyValue.ToString());

            return Control;
        }

        protected override void ApplyReadOnly() {
            if (Control != null) {
                Control.Enabled = AllowEdit;
            }
        }

        public override void BreakLinksToControl(bool unwireEventsOnly) {
            if (Control != null) {
                Control.ValueChanged -= ExtendedEditValueChangedHandler;
            }

            if (_listBoxTemplate != null) {
                _listBoxTemplate.Dispose();
                _listBoxTemplate = null;
            }

            base.BreakLinksToControl(unwireEventsOnly);
        }

        public void Setup(IObjectSpace objectSpace, XafApplication application) {
            Helper = new WebLookupEditorHelper(application, objectSpace, MemberInfo.MemberTypeInfo, Model);
            ObjectSpace = objectSpace;
        }

        protected override void Dispose(bool disposing) {
            try {
                if (disposing) {
                    if (_listBoxTemplate != null) {
                        _listBoxTemplate.Dispose();
                        _listBoxTemplate = null;
                    }
                    if (Control != null) {
                        Control.Dispose();
                        Control = null;
                    }
                }
            } finally {
                base.Dispose(disposing);
            }
        }
    }
}