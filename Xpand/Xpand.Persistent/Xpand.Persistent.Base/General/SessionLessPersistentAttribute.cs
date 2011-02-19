using System;

namespace Xpand.Persistent.Base.General {
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class SessionLessPersistentAttribute:Attribute {
    }
}
