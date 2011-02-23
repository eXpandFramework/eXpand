using System;
using Xpand.Persistent.Base.General;

namespace Xpand.ExpressApp.Attributes {
    public class NumericFormatAttribute : Attribute, ICustomAttribute {
        string ICustomAttribute.Name {
            get { return "EditMaskAttribute;DisplayFormatAttribute"; }
        }
        string ICustomAttribute.Value {
            get { return "f0;#"; }
        }
    }
}