using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using Xpand.ExpressApp.PropertyEditors;

namespace Xpand.ExpressApp.Win.Editors {
    [DesignerCategory("")]
    [ToolboxItem(false)]
    public sealed class RepositoryItemXafEnumEdit : RepositoryItemImageComboBox {
        internal const string EditorName = "XafEnumEdit";
        XafApplication _application;
        IMemberInfo _dataSourceMemberInfo;
        IModelMemberViewItem _model;
        IObjectSpace _objectSpace;
        IMemberInfo _propertyMemberInfo;

        static RepositoryItemXafEnumEdit() {
            Register();
        }

        public RepositoryItemXafEnumEdit() {
            ReadOnly = true;
            TextEditStyle = TextEditStyles.Standard;
            ShowDropDown = ShowDropDown.SingleClick;
        }

        public override string EditorTypeName {
            get { return EditorName; }
        }

        public XafApplication Application {
            get { return _application; }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
        }

        public IModelMemberViewItem Model {
            get { return _model; }
        }

        public IMemberInfo PropertyMemberInfo {
            get { return _propertyMemberInfo; }
        }

        public IMemberInfo DataSourceMemberInfo {
            get { return _dataSourceMemberInfo; }
        }

        ITypeInfo GetObjectTypeInfo(IModelMemberViewItem model) {
            var objectView = model.ParentView as IModelObjectView;
            return objectView != null ? objectView.ModelClass.TypeInfo : null;
        }

        internal static void Register() {
            if (!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
                EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(XafEnumEdit),
                                                                               typeof(RepositoryItemXafEnumEdit),
                                                                               typeof(ImageComboBoxEditViewInfo),
                                                                               new ImageComboBoxEditPainter(), true,
                                                                               EditImageIndexes.ImageComboBoxEdit,
                                                                               typeof(PopupEditAccessible)));
            }
        }

        protected override RepositoryItem CreateRepositoryItem() {
            return new RepositoryItemXafEnumEdit();
        }

        public override void Assign(RepositoryItem item) {
            var source = item as RepositoryItemXafEnumEdit;
            if (source != null) {
                _application = source._application;
                _objectSpace = source._objectSpace;
                _model = source._model;
                _propertyMemberInfo = source._propertyMemberInfo;
                _dataSourceMemberInfo = source._dataSourceMemberInfo;
            }
            base.Assign(item);
        }

        public override BaseEdit CreateEditor() {
            return base.CreateEditor() as XafEnumEdit;
        }

        public void Init(Type type) {
            var loader = new EnumImagesLoader(type);
            Items.AddRange(loader.GetImageComboBoxItems());
            if (loader.Images.Images.Count > 0) {
                SmallImages = loader.Images;
            }
        }

        public void Setup(XafApplication application, IObjectSpace objectSpace, IModelMemberViewItem model) {
            _application = application;
            _objectSpace = objectSpace;
            _model = model;
            _propertyMemberInfo = null;
            _dataSourceMemberInfo = null;
            ITypeInfo typeInfo = GetObjectTypeInfo(model);
            if (typeInfo == null) return;
            _propertyMemberInfo = typeInfo.FindMember(model.PropertyName);
            if (!String.IsNullOrEmpty(model.DataSourceProperty)) {
                var builder = new StringBuilder(model.DataSourceProperty);
                IList<IMemberInfo> path = _propertyMemberInfo.GetPath();
                for (int index = path.Count - 2; index >= 0; index--)
                    builder.Insert(0, ".").Insert(0, path[index].Name);
                _dataSourceMemberInfo = typeInfo.FindMember(builder.ToString());
            }
            Init(_propertyMemberInfo.MemberType);
        }
    }

    [DesignerCategory("")]
    [ToolboxItem(false)]
    public partial class XafEnumEdit : ImageComboBoxEdit, IGridInplaceEdit {
        static PropertyDescriptorCollection _imageComboBoxItemProperties;
        object _gridEditingObject;
        IObjectSpace _objectSpace;

        static XafEnumEdit() {
            RepositoryItemXafEnumEdit.Register();
        }

        public XafEnumEdit() {
            Properties.TextEditStyle = TextEditStyles.Standard;
            Properties.ReadOnly = true;
            Height = WinPropertyEditor.TextControlHeight;
        }

        protected static PropertyDescriptorCollection ImageComboBoxItemProperties {
            get {
                return _imageComboBoxItemProperties ??
                       (_imageComboBoxItemProperties = TypeDescriptor.GetProperties(typeof(ImageComboBoxItem)));
            }
        }

        public override string EditorTypeName {
            get { return RepositoryItemXafEnumEdit.EditorName; }
        }

        public object EditingObject {
            get { return BindingHelper.GetEditingObject(this); }
        }

        public new RepositoryItemXafEnumEdit Properties {
            get { return (RepositoryItemXafEnumEdit)base.Properties; }
        }

        public object GridEditingObject {
            get { return _gridEditingObject; }
            set {
                if (_gridEditingObject == value) return;
                _gridEditingObject = value;
            }
        }

        public IObjectSpace ObjectSpace {
            get { return _objectSpace; }
            set {
                if (_objectSpace != null) _objectSpace.ObjectChanged -= ObjectSpaceObjectChanged;
                _objectSpace = value;
                if (_objectSpace != null) _objectSpace.ObjectChanged += ObjectSpaceObjectChanged;
            }
        }

        public new XafEnumEditPopupForm PopupForm {
            get { return (XafEnumEditPopupForm)base.PopupForm; }
        }

        internal IList GetDataSource(object forObject) {
            CriteriaOperator criteria = null;
            if (Properties == null) return null;
            IList propertyDataSource =
                (Properties.DataSourceMemberInfo != null) &&
                (Properties.DataSourceMemberInfo.IsStatic || GridEditingObject != null)
                    ? Properties.DataSourceMemberInfo.GetValue(forObject) as IList
                    : null;
            IList dataSource = new List<ImageComboBoxItem>();
            if (propertyDataSource == null)
                for (int i = 0; i < Properties.Items.Count; i++)
                    dataSource.Add(Properties.Items[i]);
            else
                for (int i = 0; i < Properties.Items.Count; i++) {
                    ImageComboBoxItem item = Properties.Items[i];
                    if (propertyDataSource.Contains(item.Value))
                        dataSource.Add(item);
                }
            string criteriaString = Properties.Model.DataSourceCriteria;
            if (!String.IsNullOrEmpty(criteriaString))
                criteria = CriteriaOperator.Parse(criteriaString);
            if (!ReferenceEquals(criteria, null)) {
                criteria.Accept(new EnumCriteriaParser(
                                    Properties.PropertyMemberInfo.Name,
                                    Properties.PropertyMemberInfo.MemberType));
                ICollection filteredDataSource =
                    new ExpressionEvaluator(ImageComboBoxItemProperties, criteria, true).Filter(dataSource);
                dataSource.Clear();
                foreach (ImageComboBoxItem item in filteredDataSource)
                    dataSource.Add(item);
            }
            return dataSource;
        }

        void ObjectSpaceObjectChanged(object sender, ObjectChangedEventArgs e) {
            if (e.Object == GridEditingObject && (String.IsNullOrEmpty(e.PropertyName) || (Properties.DataSourceMemberInfo != null && Properties.DataSourceMemberInfo.Name.Equals(e.PropertyName)))) {

            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                ClosePopup();
            }
            base.OnKeyDown(e);
        }

        protected override void OnPropertiesChanged() {
            base.OnPropertiesChanged();
            if (Properties != null)
                ObjectSpace = Properties.ObjectSpace;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                ObjectSpace = null;
            }
            base.Dispose(disposing);
        }

        protected override PopupBaseForm CreatePopupForm() {
            return new XafEnumEditPopupForm(this);
        }
    }

    public partial class XafEnumEdit {
    }

    public class XafEnumEditPopupForm : PopupListBoxForm {


        public XafEnumEditPopupForm(XafEnumEdit ownerEdit)
            : base(ownerEdit) {
        }



        public new XafEnumEdit OwnerEdit {
            get { return (XafEnumEdit)base.OwnerEdit; }
        }

        public new RepositoryItemXafEnumEdit Properties {
            get { return (RepositoryItemXafEnumEdit)base.Properties; }
        }

        protected override void OnBeforeShowPopup() {
            UpdateDataSource();
            base.OnBeforeShowPopup();
        }

        protected override void SetupListBoxOnShow() {
            base.SetupListBoxOnShow();
            var visibleItems = ListBox.DataSource as IList;
            var currentItem = (ImageComboBoxItem)OwnerEdit.SelectedItem;
            bool currentItemInVisibleItems = visibleItems == null || visibleItems.Contains(currentItem);
            var selectedItem = (ImageComboBoxItem)ListBox.SelectedItem;
            if (selectedItem != currentItem || !currentItemInVisibleItems)
                selectedItem = null;

            if (selectedItem == null && currentItemInVisibleItems)
                selectedItem = currentItem;

            ListBox.SelectedIndex = -1;

            ListBox.SelectedItem = selectedItem;
        }

        public void UpdateDataSource() {
            if (Properties == null) return;

            IList dataSource = OwnerEdit.GetDataSource(OwnerEdit.EditingObject);

            ListBox.DataSource = dataSource ?? Properties.Items;
        }
    }
}