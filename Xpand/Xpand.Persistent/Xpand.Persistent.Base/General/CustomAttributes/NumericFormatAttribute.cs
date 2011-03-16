using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    public class NumericFormatAttribute : Attribute, ICustomAttribute {
        string ICustomAttribute.Name {
            get { return "EditMaskAttribute;DisplayFormatAttribute"; }
        }
        string ICustomAttribute.Value {
            get { return "f0;#"; }
        }
    }
}