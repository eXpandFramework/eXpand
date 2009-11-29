using System;

namespace eXpand.Persistent.Base.PersistentMetaData {
    public interface IPersistentMemberInfo : IPersistentTypeInfo
    {    
        IPersistentClassInfo Owner { get; set; }
    }
}