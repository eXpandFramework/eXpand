using System;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]
    public class PropertyEditorProperty:Attribute {
        readonly string _propertyName;

        public PropertyEditorProperty(string propertyName) {
            _propertyName = propertyName;
        }

        public string PropertyName {
            get { return _propertyName; }
        }
    }
}
