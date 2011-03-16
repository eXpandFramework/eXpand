using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    public class DisplayFormatAttribute : Attribute, ICustomAttribute {
        readonly string _value;

        public DisplayFormatAttribute(string value) {
            _value = value;
        }

        string ICustomAttribute.Name {
            get { return "DisplayFormat"; }
        }

        string ICustomAttribute.Value {
            get { return _value; }
        }
    }
}