using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IExtendedMemberInfo : IPersistentTypeInfo
    {
        Type Owner { get; set; }
        
    }
}