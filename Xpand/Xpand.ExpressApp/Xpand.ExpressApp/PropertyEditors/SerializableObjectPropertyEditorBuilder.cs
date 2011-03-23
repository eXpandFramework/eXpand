using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.PropertyEditors {
    public interface ISupportControl {
        object Control { get; set; }
    }
    public interface ISupportEditControl {
        ISupportControl GetControl();
    }

    public class SerializableObjectPropertyEditorBuilder {
        PropertyEditor _propertyEditor;

        public static SerializableObjectPropertyEditorBuilder Create() {
            return new SerializableObjectPropertyEditorBuilder();
        }

        public SerializableObjectPropertyEditorBuilder WithPropertyEditor(PropertyEditor propertyEditor) {
            _propertyEditor = propertyEditor;
            return this;
        }
        void OnControlCreated(object sender, EventArgs eventArgs) {
            UpdateEditor(((ISupportEditControl)_propertyEditor).GetControl());
        }

        void OnValueStoring(object sender, ValueStoringEventArgs valueStoringEventArgs) {
            _propertyEditor.PropertyValue = _parameter.CurrentValue;
        }

        void OnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            _propertyEditor.View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            UpdateEditor(((ISupportEditControl)_propertyEditor).GetControl());
        }
        public void Build(Func<PropertyEditor,object> findControl) {
            _findControl = findControl;
            _propertyEditor.CurrentObjectChanged += OnCurrentObjectChanged;
            _propertyEditor.ValueStoring += OnValueStoring;
            _propertyEditor.ControlCreated += OnControlCreated;
        }
        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            UpdateEditor(((ISupportEditControl)_propertyEditor).GetControl());
        }
        Parameter _parameter;
        PropertyEditor _detailViewItems;
        Func<XafApplication> _getApplicationAction;
        Func<PropertyEditor,object> _findControl;

        void UpdateEditor(ISupportControl supportControl) {
            if (supportControl==null)
                return;
            bool isChanged = false;
            var memberType = GetMemberType() ?? typeof(object);
            bool editObjectChanged = (_parameter != null) && (_parameter.Type != memberType);
            if (_propertyEditor.CurrentObject != null) {
                if ((_parameter == null) || (editObjectChanged) || supportControl.Control==null) {
                    var application = _getApplicationAction.Invoke();
                    isChanged = true;
                    _parameter = new Parameter(memberType.Name, memberType);
                    var paramList = new ParameterList { _parameter };
                    ParametersObject parametersObject = ParametersObject.CreateBoundObject(paramList);
                    DetailView detailView = parametersObject.CreateDetailView(application.CreateObjectSpace(), application, true);
                    detailView.ViewEditMode = GetViewEditMode();
                    _detailViewItems = ((PropertyEditor)detailView.Items[0]);
                    _detailViewItems.CreateControl();
                    _detailViewItems.ControlValueChanged += detailViewItems_ControlValueChanged;
                }
                _parameter.CurrentValue = _propertyEditor.PropertyValue;
            }
            if ((isChanged || (supportControl.Control == null)) && (_detailViewItems != null)) {
                _detailViewItems.Refresh();
                supportControl.Control = _findControl.Invoke(_detailViewItems);
            }
        }

        ViewEditMode GetViewEditMode() {
            if (_propertyEditor.View is DetailView)
                return ((DetailView)_propertyEditor.View).ViewEditMode;
            throw new NotImplementedException();
        }

        Type GetMemberType() {
            return (Type)_propertyEditor.View.ObjectTypeInfo.FindMember(_propertyEditor.MemberInfo.FindAttribute<PropertyEditorProperty>().PropertyName).GetValue(_propertyEditor.CurrentObject);
        }

        void detailViewItems_ControlValueChanged(object sender, EventArgs e) {
            _propertyEditor.PropertyValue = _detailViewItems.ControlValue;
        }

        public SerializableObjectPropertyEditorBuilder WithApplication(Func<XafApplication> getApplicationAction) {
            _getApplicationAction = getApplicationAction;
            return this;
        }
    }
}