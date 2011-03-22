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
            UpdateEditor(_propertyEditor.Control as ISupportControl);
        }

        void OnValueStoring(object sender, ValueStoringEventArgs valueStoringEventArgs) {
            _propertyEditor.PropertyValue = param.CurrentValue;
        }

        void OnCurrentObjectChanged(object sender, EventArgs eventArgs) {
            _propertyEditor.View.ObjectSpace.ObjectChanged += ObjectSpaceOnObjectChanged;
            UpdateEditor(_propertyEditor.Control as ISupportControl);
        }
        public void Build() {
            _propertyEditor.CurrentObjectChanged += OnCurrentObjectChanged;
            _propertyEditor.ValueStoring += OnValueStoring;
            _propertyEditor.ControlCreated += OnControlCreated;
        }
        void ObjectSpaceOnObjectChanged(object sender, ObjectChangedEventArgs objectChangedEventArgs) {
            UpdateEditor(_propertyEditor.Control as ISupportControl);
        }
        Parameter param;
        PropertyEditor detailViewItems;
        Func<XafApplication> _getApplicationAction;

        void UpdateEditor(ISupportControl supportControl) {
            if (supportControl==null)
                return;
            bool isChanged = false;
            var memberType = GetMemberType() ?? typeof(object);
            bool editObjectChanged = (param != null) && (param.Type != memberType);
            if (_propertyEditor.CurrentObject != null) {
                if ((param == null) || (editObjectChanged)) {
                    var application = _getApplicationAction.Invoke();
                    isChanged = true;
                    param = new Parameter(memberType.Name, memberType);
                    var paramList = new ParameterList { param };
                    ParametersObject parametersObject = ParametersObject.CreateBoundObject(paramList);
                    DetailView detailView = parametersObject.CreateDetailView(application.CreateObjectSpace(), application, true);
                    detailViewItems = ((PropertyEditor)detailView.Items[0]);
                    detailViewItems.CreateControl();
                    detailViewItems.ControlValueChanged += detailViewItems_ControlValueChanged;
                }
                param.CurrentValue = _propertyEditor.PropertyValue;
            }
            if ((isChanged || (supportControl.Control == null)) && (detailViewItems != null)) {
                detailViewItems.Refresh();
                supportControl.Control = detailViewItems.Control ;
            }
        }

        Type GetMemberType() {
            return (Type)_propertyEditor.View.ObjectTypeInfo.FindMember(_propertyEditor.MemberInfo.FindAttribute<PropertyEditorProperty>().PropertyName).GetValue(_propertyEditor.CurrentObject);
        }

        void detailViewItems_ControlValueChanged(object sender, EventArgs e) {
            _propertyEditor.PropertyValue = detailViewItems.ControlValue;
        }

        public SerializableObjectPropertyEditorBuilder WithApplication(Func<XafApplication> getApplicationAction) {
            _getApplicationAction = getApplicationAction;
            return this;
        }
    }
}