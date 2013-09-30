using System;

namespace Xpand.ExpressApp.Attributes {
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false,Inherited = false)]
    public class InvisibleInAllViewsAttribute:Attribute {
    }
}
