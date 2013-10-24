using System;

namespace Xpand.Persistent.Base.General.CustomAttributes {
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false,Inherited = false)]
    public class InvisibleInAllViewsAttribute:Attribute {
    }
}
