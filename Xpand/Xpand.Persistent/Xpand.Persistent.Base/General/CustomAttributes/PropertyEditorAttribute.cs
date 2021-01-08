using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyEditorAttribute : Attribute, ICustomAttribute {
        readonly Type _propertyEditorType;

        public PropertyEditorAttribute(Type propertyEditorType) {
            _propertyEditorType = propertyEditorType;
        }

        public string Name => "PropertyEditorType";

        public string Value => _propertyEditorType.FullName;
    }
}
