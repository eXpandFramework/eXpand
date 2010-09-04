using System;

namespace Xpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedMemberInfo : IPersistentTypeInfo
    {
        Type Owner { get; set; }
        
    }
}