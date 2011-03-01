using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    public class DisplayDateAndTime : Attribute, ICustomAttribute {
        string ICustomAttribute.Name {
            get { return "DisplayFormat;EditMask"; }
        }

        string ICustomAttribute.Value {
            get { return "{0: ddd, dd MMMM yyyy hh:mm:ss tt};ddd, dd MMMM yyyy hh:mm:ss tt"; }
        }
    }
}