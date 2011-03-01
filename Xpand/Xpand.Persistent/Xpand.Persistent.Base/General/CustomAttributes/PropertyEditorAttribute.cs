using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PropertyEditorAttribute : Attribute, ICustomAttribute {
        readonly Type _propertyEditorType;

        public PropertyEditorAttribute(Type propertyEditorType) {
            _propertyEditorType = propertyEditorType;
        }

        string ICustomAttribute.Name {
            get { return "PropertyEditorType"; }
        }

        string ICustomAttribute.Value {
            get { return _propertyEditorType.FullName; }
        }
    }
}
